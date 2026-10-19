package com.fireman.port;

import java.util.ArrayList;
import java.util.List;

/**
 * The playable simulation, ported from the original Unity scripts.
 *
 * Coordinate system: UI pixels, y up. This is the space the original works in
 * once NGUI's UI root scale (1/360, manualHeight 720) is folded in, so every
 * constant below is the original value multiplied by 360:
 *
 *   gravity   Physics.gravity.y = -9 world units      -> -3240 px/s^2
 *   jump      GamePhysics.FIREMAN_JUMP_VEL = 3        ->  1080 px/s
 *   jetpack   GamePhysics.FIREMAN_JET_VEL = 0.96      ->   345.6 px/s
 *   walk      Pompier.MoveLeft/MoveRight = 1          ->   360 px/s
 *   tile      LevelLoader.scaleX step = 50            ->    50 px
 *   collider  pompier.prefab BoxCollider = 40 x 60    ->    40 x 60 px
 *   water     Pompier.addGoutte speed = 2             ->   720 px/s
 *
 * Those numbers are self-consistent: a jump peaks at 180 px, i.e. 3.6 tiles,
 * which is what the level layouts assume, and the 52 px decor sprites (from the
 * decor prefab's localScale) tile the 50 px grid.
 *
 * No Android types are used here so the whole simulation can be driven by a
 * plain JVM test.
 */
public final class GameWorld {

    public static final float TILE = 50f;
    public static final float GRAVITY = 3240f;
    public static final float JUMP_VEL = 1080f;
    public static final float JET_VEL = 345.6f;
    public static final float WALK_VEL = 360f;
    public static final float WATER_VEL = 720f;
    public static final float PLAYER_HALF_W = 20f;
    public static final float PLAYER_HALF_H = 30f;
    /**
     * pompier.prefab's BoxCollider has m_Center {0, 5, 0}, so the collider sits
     * 5 px above the transform origin. The level grid places the player's origin
     * exactly on the floor surface, so without this offset the spawn overlaps
     * the ground and the collider resolution ejects the player downward.
     */
    public static final float COLLIDER_CY = 5f;
    /** Patrol speed of the walking enemies, in world px/s. */
    public static final float MOB_PATROL_VEL = 90f;
    /** NGUI draws a decor sprite as a unit quad scaled by the prefab's 52. */
    public static final float TILE_SPRITE = 52f;

    /** Touch/contact radii, straight from the original prefabs and scripts. */
    private static final float PICKUP_DX = 30f, PICKUP_DY = 30f;
    private static final float VICTIM_DX = 40f, VICTIM_DY = 40f;
    private static final float EXIT_DX = 20f, EXIT_DY = 20f;

    private static final float STEP = 1f / 60f;
    private static final float MAX_STEP = 1f / 15f;

    /** Water reserve drains while firing and refills while idle. */
    private static final float WATER_DRAIN = 0.5f;
    private static final float WATER_REFILL = 0.25f;

    public static final int PLAYING = 0, DEAD = 1, WON = 2, GAMEOVER = 3;

    // ---- entity state -------------------------------------------------------

    public static final class Solid {
        public float left, top, right, bottom;
    }

    /** Anything the player can shoot, touch, or collect. */
    public static final class Mob {
        public int type;
        public float x, y, vx, vy, w, h;
        public boolean onGround;
        public float health = 1f;
        public boolean gone;
        /** Counts down while the extinguished fire shows its ash animation. */
        public float ashTimer;
        public float homeX, homeY;
        public int patrolDir = 1;
        public float patrolRange = 100f;
        public float phase;
    }

    public static final class Pickup {
        public int type;          // T_COIN, T_TANK, T_VICTIM
        public float x, y;
        public boolean gone;
    }

    public static final class Platform {
        public float x, y, y0, y1, speed, phase;
        public boolean vertical;
        public float prevY;
    }

    public static final class Water {
        public float x, y, vx, vy, life;
        public boolean alive;
    }

    public static final class Teleport {
        public float x, y, dx, dy;
    }

    // ---- public state -------------------------------------------------------

    public final List<Solid> solids = new ArrayList<>();
    public final List<Mob> mobs = new ArrayList<>();
    public final List<Pickup> pickups = new ArrayList<>();
    public final List<Platform> platforms = new ArrayList<>();
    public final List<Water> water = new ArrayList<>();
    public final List<Teleport> teleports = new ArrayList<>();

    public final LevelData level;
    public final int world;

    public float px, py, pvx, pvy;
    public boolean onGround, facingRight = true;
    public float aimY = 0.2f;
    public int life = 3;
    public int status = PLAYING;
    public int flamesOut, coinsCollected, victimsSaved;
    public float reserveWater = 1f;
    public float spawnX, spawnY;
    public float deathTimer;
    public int levelIndex;

    public boolean inputLeft, inputRight, inputJump, inputShoot;

    /** Level envelope, derived from the box colliders; the player is clamped to it. */
    public float minX, maxX;

    /** Seconds since the level started; drives sprite animation. */
    public float time;

    private int teleportCooldown;

    public GameWorld(LevelData level, int world, int levelIndex) {
        this.level = level;
        this.world = world;
        this.levelIndex = levelIndex;
        build();
    }

    private void build() {
        minX = Float.MAX_VALUE;
        maxX = -Float.MAX_VALUE;
        for (LevelData.Box b : level.boxes) {
            Solid s = new Solid();
            s.left = b.left();
            s.right = b.right();
            s.top = b.top();
            s.bottom = b.bottom();
            solids.add(s);
            minX = Math.min(minX, s.left);
            maxX = Math.max(maxX, s.right);
        }
        if (solids.isEmpty()) {
            minX = -500f;
            maxX = 500f;
        }

        for (LevelData.Entity e : level.entities) {
            if (!e.mode) continue;
            float x = e.worldX(), y = e.worldY();
            switch (e.type) {
                case LevelData.T_FIREMAN:
                    if (spawnX == 0 && spawnY == 0) {
                        spawnX = px = x;
                        spawnY = py = y;
                    }
                    break;
                case LevelData.T_FLAMMER:
                    mobs.add(mob(LevelData.T_FLAMMER, x, y, 40, 40, 1000f));
                    break;
                case LevelData.T_CRAWLER: {
                    Mob m = mob(LevelData.T_CRAWLER, x, y, 40, 50, 900f);
                    m.patrolRange = e.dist != 0 ? Math.abs(e.dist) * TILE : 200f;
                    break;
                }
                case LevelData.T_RUNNER:
                    mobs.add(mob(LevelData.T_RUNNER, x, y, 40, 50, 900f));
                    break;
                case LevelData.T_BOMBER:
                    mobs.add(mob(LevelData.T_BOMBER, x, y, 40, 50, 900f));
                    break;
                case LevelData.T_THROWER:
                    mobs.add(mob(LevelData.T_THROWER, x, y, 40, 50, 900f));
                    break;
                case LevelData.T_SPIDER:
                    mobs.add(mob(LevelData.T_SPIDER, x, y, 40, 50, 900f));
                    break;
                case LevelData.T_BOSS1:
                    mobs.add(mob(LevelData.T_BOSS1, x, y, 90, 100, 900f));
                    break;
                case LevelData.T_COIN:
                    pickups.add(pickup(LevelData.T_COIN, x, y));
                    break;
                case LevelData.T_TANK:
                    pickups.add(pickup(LevelData.T_TANK, x, y));
                    break;
                case LevelData.T_VICTIM:
                    pickups.add(pickup(LevelData.T_VICTIM, x, y));
                    break;
                case LevelData.T_UPDOWN: {
                    Platform p = new Platform();
                    p.x = x;
                    p.y0 = y;
                    // range is a half-travel in grid rows in the original data
                    p.y1 = y + Math.abs(e.range) * TILE;
                    p.speed = e.speed != 0 ? Math.abs(e.speed) * TILE : 100f;
                    p.y = p.y0;
                    p.prevY = p.y;
                    p.vertical = true;
                    p.phase = e.index * 0.1f;
                    platforms.add(p);
                    break;
                }
                case LevelData.T_TELEPORT: {
                    Teleport t = new Teleport();
                    t.x = x;
                    t.y = y;
                    t.dx = LevelData.scaleX(e.dx);
                    t.dy = LevelData.scaleY(e.dy);
                    teleports.add(t);
                    break;
                }
                default:
                    break; // tiles are drawn from the level, hydrants are decor
            }
        }
        if (spawnX == 0 && spawnY == 0) {
            spawnX = px = LevelData.scaleX(0);
            spawnY = py = LevelData.scaleY(0);
        }
    }

    private static Mob mob(int type, float x, float y, float w, float h, float patrol) {
        Mob m = new Mob();
        m.type = type;
        m.x = m.homeX = x;
        m.y = m.homeY = y;
        m.w = w;
        m.h = h;
        m.patrolRange = patrol;
        return m;
    }

    private static Pickup pickup(int type, float x, float y) {
        Pickup p = new Pickup();
        p.type = type;
        p.x = x;
        p.y = y;
        return p;
    }

    // ---- simulation ---------------------------------------------------------

    /** Advances by real elapsed seconds, split into the original's 60 Hz steps. */
    public void update(float elapsed) {
        float left = Math.min(elapsed, MAX_STEP);
        while (left > 0f) {
            float dt = Math.min(STEP, left);
            step(dt);
            left -= dt;
        }
    }

    private void step(float dt) {
        time += dt;
        updatePlatforms(dt);
        for (Mob m : mobs) if (m.ashTimer > 0f) m.ashTimer -= dt;
        if (status == PLAYING) {
            updatePlayer(dt);
            updateMobs(dt);
            updateWater(dt);
            updatePickups();
            updateTeleports();
        } else if (status == DEAD) {
            deathTimer -= dt;
            if (deathTimer <= 0f) {
                if (life <= 0) status = GAMEOVER;
                else respawn();
            }
        }
    }

    private void updatePlatforms(float dt) {
        for (Platform p : platforms) {
            p.prevY = p.y;
            float span = p.y1 - p.y0;
            if (span <= 0f) continue;
            p.phase += (p.speed / span) * dt;
            float t = (p.phase % 2f + 2f) % 2f;
            float u = t < 1f ? t : 2f - t;
            p.y = p.y0 + span * u;
        }
    }

    private void updatePlayer(float dt) {
        // Aim drifts back to the original's 0.2 default, as in Pompier.Update.
        if (aimY < 0.2f) aimY = Math.min(0.2f, aimY + dt);
        else if (aimY > 0.2f) aimY = Math.max(0.2f, aimY - dt);

        // Pompier.MoveLeft/MoveRight: full speed on the ground, 0.9x in the air.
        float walk = onGround ? WALK_VEL : WALK_VEL * 0.9f;
        if (inputLeft) { facingRight = false; pvx = -walk; }
        else if (inputRight) { facingRight = true; pvx = walk; }
        else pvx = 0f;

        boolean jet = false;
        if (inputJump) {
            if (onGround) {
                pvy = JUMP_VEL;
                onGround = false;
            } else if (pvy <= 0f) {
                pvy = JET_VEL;
                jet = true;
            }
        }

        pvy -= GRAVITY * dt;
        if (pvy < -2000f) pvy = -2000f;

        movePlayer(pvx * dt, pvy * dt);

        if (inputShoot) {
            if (reserveWater > 0f) shoot();
            reserveWater = Math.max(0f, reserveWater - WATER_DRAIN * dt);
        } else {
            reserveWater = Math.min(1f, reserveWater + WATER_REFILL * dt);
        }
        // The jetpack also sprays, which is why it costs water in the original.
        if (jet) reserveWater = Math.max(0f, reserveWater - WATER_DRAIN * dt);
    }

    private void movePlayer(float dx, float dy) {
        // Axis-separated resolution. Only the axis that actually moved is
        // resolved, so an overlap that already existed on the other axis cannot
        // shove the player sideways.
        if (dx != 0f) {
            float prevLeft = px - PLAYER_HALF_W, prevRight = px + PLAYER_HALF_W;
            px += dx;
            for (Solid s : solids) {
                if (overlaps(px, py, s)) {
                    if (dx > 0f && prevRight <= s.left + 1f) px = s.left - PLAYER_HALF_W;
                    else if (dx < 0f && prevLeft >= s.right - 1f) px = s.right + PLAYER_HALF_W;
                    else px = (px < (s.left + s.right) / 2f) ? s.left - PLAYER_HALF_W
                                                             : s.right + PLAYER_HALF_W;
                    pvx = 0f;
                }
            }
        }

        if (dy != 0f) {
            float prevTop = py + COLLIDER_CY + PLAYER_HALF_H;
            float prevBottom = py + COLLIDER_CY - PLAYER_HALF_H;
            py += dy;
            onGround = false;
            for (Solid s : solids) {
                if (overlaps(px, py, s)) {
                    if (dy <= 0f && prevBottom >= s.top - 1f) {
                        py = s.top - COLLIDER_CY + PLAYER_HALF_H;
                        pvy = 0f;
                        onGround = true;
                    } else if (dy > 0f && prevTop <= s.bottom + 1f) {
                        py = s.bottom - COLLIDER_CY - PLAYER_HALF_H;
                        pvy = 0f;
                    } else {
                        py = (py + COLLIDER_CY < (s.bottom + s.top) / 2f)
                                ? s.bottom - COLLIDER_CY - PLAYER_HALF_H
                                : s.top - COLLIDER_CY + PLAYER_HALF_H;
                        pvy = 0f;
                    }
                }
            }
        }

        // Keep the player inside the level envelope; the colliders define the
        // playable area and the original never lets the fireman leave it.
        if (px < minX + PLAYER_HALF_W) { px = minX + PLAYER_HALF_W; pvx = 0f; }
        if (px > maxX - PLAYER_HALF_W) { px = maxX - PLAYER_HALF_W; pvx = 0f; }

        // Moving platforms carry the player when standing on them.
        for (Platform p : platforms) {
            float pl = p.x - TILE / 2f, pr = p.x + TILE / 2f;
            float pt = p.y + TILE / 2f;
            if (px + PLAYER_HALF_W > pl && px - PLAYER_HALF_W < pr
                    && Math.abs(py + COLLIDER_CY - PLAYER_HALF_H - pt) < 6f && pvy <= 0f) {
                py = pt - COLLIDER_CY + PLAYER_HALF_H;
                pvy = 0f;
                onGround = true;
                py += (p.y - p.prevY);
            }
        }
    }

    private static boolean overlaps(float cx, float cy, Solid s) {
        float ccy = cy + COLLIDER_CY;
        return cx + PLAYER_HALF_W > s.left && cx - PLAYER_HALF_W < s.right
                && ccy + PLAYER_HALF_H > s.bottom && ccy - PLAYER_HALF_H < s.top;
    }

    /**
     * Spawns a drop exactly as Pompier.addGoutte does:
     *   vel = (jitter + shotDir.x * 2 + playerVel.x, jitter + shotDir.y * 2)
     *   pos = player + shotDir * (ofsShootX, ofsShootY)
     * shotDir.y is the aim value, which the original clamps to [0.2, 1].
     *
     * The caller invokes this twice per shot (addGoutte(1) and addGoutte(0));
     * the extra Update() on the first copy is a sub-frame detail we skip.
     */
    private void shoot() {
        float dirX = facingRight ? 1f : -1f;
        float ax = dirX;
        float ay = aimY < 0.2f ? 0.2f : (aimY > 1f ? 1f : aimY);
        float jitterX = rnd.nextFloat() * 0.1f;
        float jitterY = rnd.nextFloat() * 0.1f;
        for (int i = 0; i < 2; i++) {
            Water w = new Water();
            w.x = px + ax * 35f;
            w.y = py + ay * 1f;
            w.vx = jitterX + ax * 2f + pvx / 360f;
            w.vy = jitterY + ay * 2f;
            w.life = 1f;
            w.alive = true;
            water.add(w);
        }
    }

    private final java.util.Random rnd = new java.util.Random();

    private void updateWater(float dt) {
        for (int i = water.size() - 1; i >= 0; i--) {
            Water w = water.get(i);
            // Goutte.Update integrates velocity in world units then scales by
            // uiScaleX/uiScaleY, i.e. 360 px per unit.
            w.vy -= 9f * dt;
            w.x += w.vx * 360f * dt;
            w.y += w.vy * 360f * dt;
            w.life -= dt;
            if (w.life <= 0f) { water.remove(i); continue; }

            boolean hitSomething = false;
            for (Mob m : mobs) {
                if (m.gone) continue;
                // Goutte tests |dx| < ColX + 8 and |dy| < ColY + 8 with ColX/Y 25.
                if (Math.abs(w.x - m.x) < m.w / 2f + 8f && Math.abs(w.y - m.y) < m.h / 2f + 8f) {
                    damage(m);
                    hitSomething = true;
                    break;
                }
            }
            if (!hitSomething) {
                for (Solid s : solids) {
                    if (w.x > s.left && w.x < s.right && w.y > s.bottom && w.y < s.top) {
                        hitSomething = true;
                        break;
                    }
                }
            }
            if (hitSomething) water.remove(i);
        }
    }

    private void damage(Mob m) {
        // Ennemi1 loses 0.2 per hit; the others lose 0.1. One hit is enough for
        // everything except fire, which takes five.
        float dmg = m.type == LevelData.T_FLAMMER ? 0.2f : 0.1f;
        m.health -= dmg;
        if (m.health <= 0f) {
            m.health = 0f;
            m.gone = true;
            // The original shows an ash puff (Cendre01..05) for about a second.
            m.ashTimer = 1.2f;
            flamesOut++;
        }
    }

    private void updateMobs(float dt) {
        for (Mob m : mobs) {
            if (m.gone) continue;
            if (m.type != LevelData.T_FLAMMER && m.type != LevelData.T_BOSS1) {
                m.x += m.patrolDir * MOB_PATROL_VEL * dt;
                if (m.x > m.homeX + m.patrolRange) m.patrolDir = -1;
                if (m.x < m.homeX - m.patrolRange) m.patrolDir = 1;

                // Patrol enemies are gravity-bound: integrate velocity, then snap
                // onto the first collider below so they walk the floor instead of
                // sinking through it.
                m.vy -= GRAVITY * dt;
                float nextY = m.y + m.vy * dt;
                float floor = -Float.MAX_VALUE;
                for (Solid s : solids) {
                    if (m.x + m.w / 2f > s.left && m.x - m.w / 2f < s.right
                            && s.top <= m.y + 1f && s.top > floor) {
                        floor = s.top;
                    }
                }
                if (m.vy <= 0f && floor > -Float.MAX_VALUE && nextY - m.h / 2f <= floor) {
                    nextY = floor + m.h / 2f;
                    m.vy = 0f;
                    m.onGround = true;
                } else {
                    m.onGround = false;
                }
                m.y = nextY;
            }

            // Contact with the player is fatal. Enemy colliders are 25 half-width
            // (BaseEntity.ColX/ColY) and the player's is 20x30.
            float kx = m.w / 2f + PLAYER_HALF_W;
            float ky = m.h / 2f + PLAYER_HALF_H;
            if (Math.abs(px - m.x) < kx && Math.abs(py - m.y) < ky) {
                killPlayer();
                return;
            }
        }
    }

    private void updatePickups() {
        for (Pickup p : pickups) {
            if (p.gone) continue;
            if (p.type == LevelData.T_VICTIM) {
                if (Math.abs(px - p.x) < VICTIM_DX && Math.abs(py - p.y) < VICTIM_DY) {
                    p.gone = true;
                    victimsSaved++;
                }
            } else {
                if (Math.abs(px - p.x) < PICKUP_DX && Math.abs(py - p.y) < PICKUP_DY) {
                    p.gone = true;
                    coinsCollected++;
                }
            }
        }
        // Exit wins the level.
        for (LevelData.Entity e : level.entities) {
            if (e.type != LevelData.T_EXIT || !e.mode) continue;
            if (Math.abs(px - e.worldX()) < EXIT_DX && Math.abs(py - e.worldY()) < EXIT_DY) {
                status = WON;
            }
        }
    }

    private void updateTeleports() {
        if (teleportCooldown > 0) { teleportCooldown--; return; }
        for (Teleport t : teleports) {
            if (Math.abs(px - t.x) < PICKUP_DX && Math.abs(py - t.y) < PICKUP_DY) {
                px = t.dx;
                py = t.dy;
                pvx = pvy = 0f;
                teleportCooldown = 30;
                return;
            }
        }
    }

    private void killPlayer() {
        if (status != PLAYING) return;
        status = DEAD;
        deathTimer = 1f;
        life--;
    }

    private void respawn() {
        px = spawnX;
        py = spawnY;
        pvx = pvy = 0f;
        reserveWater = 1f;
        status = PLAYING;
    }

    /** Restarts the level with full lives, for the game-over screen. */
    public void restartLevel() {
        life = 3;
        flamesOut = 0;
        coinsCollected = 0;
        victimsSaved = 0;
        for (Mob m : mobs) { m.gone = false; m.health = 1f; m.x = m.homeX; }
        for (Pickup p : pickups) p.gone = false;
        water.clear();
        respawn();
    }
}
