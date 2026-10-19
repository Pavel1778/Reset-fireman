package com.fireman.port;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Matrix;
import android.graphics.Paint;
import android.graphics.Rect;
import android.graphics.RectF;
import android.view.SurfaceHolder;

import java.util.List;

/**
 * Draws the world with the original atlas artwork.
 *
 * The view keeps a fixed 1280x720 UI space, exactly the NGUI manual height of
 * 720 the original was authored against, and letterboxes it into the surface.
 * That way one set of coordinates drives both physics and rendering.
 */
public final class Renderer {

    public static final float VIEW_H = 720f;
    public static final float VIEW_W = 1280f;

    private final Context ctx;
    private final Atlas atlas;

    private final Paint paint = new Paint(Paint.FILTER_BITMAP_FLAG);
    private final Paint text = new Paint(Paint.ANTI_ALIAS_FLAG);
    private final Rect src = new Rect();
    private final RectF dst = new RectF();
    private final Matrix m = new Matrix();

    private Bitmap bg;

    public Renderer(Context ctx, Atlas atlas) {
        this.ctx = ctx;
        this.atlas = atlas;
        text.setColor(Color.WHITE);
        text.setTextSize(28f);
        text.setShadowLayer(3f, 2f, 2f, 0xCC000000);
    }

    public void setBackground(String name) {
        try (java.io.InputStream in = ctx.getAssets().open("atlas/" + name)) {
            bg = android.graphics.BitmapFactory.decodeStream(in);
        } catch (Exception e) {
            bg = null;
        }
    }
    /** Surface pixels -> UI pixels. */
    private float scale = 1f, offX = 0f, offY = 0f;

    public void configure(SurfaceHolder holder) {
        int w = holder.getSurfaceFrame().width();
        int h = holder.getSurfaceFrame().height();
        if (w <= 0 || h <= 0) { scale = 1f; offX = offY = 0f; return; }
        scale = Math.min(w / VIEW_W, h / VIEW_H);
        offX = (w - VIEW_W * scale) / 2f;
        offY = (h - VIEW_H * scale) / 2f;
    }

    /** Camera: follows the player, clamped to the level bounds. */
    private float camX, camY;

    public void updateCamera(GameWorld g, float levelMinX, float levelMaxX) {
        camX = clamp(g.px - VIEW_W / 2f, levelMinX, Math.max(levelMinX, levelMaxX - VIEW_W));
        camY = clamp(g.py - VIEW_H / 2f, -260f, 480f);
    }

    private static float clamp(float v, float lo, float hi) {
        return v < lo ? lo : (v > hi ? hi : v);
    }

    public void draw(Canvas c, GameWorld g) {
        c.drawColor(0xFF101820);

        c.save();
        c.translate(offX, offY);
        c.scale(scale, scale);

        drawBackground(c);
        drawEntities(c, g);
        drawPlatforms(c, g);
        drawWater(c, g);
        drawMobs(c, g);
        drawPlayer(c, g);

        c.restore();
        drawHud(c, g);
    }

    private void drawBackground(Canvas c) {
        if (bg == null) return;
        // The source art is 1024x684; scale it to cover the 1280x720 view while
        // preserving its aspect so it is not visibly stretched.
        float bw = bg.getWidth(), bh = bg.getHeight();
        float s = Math.max(VIEW_W / bw, VIEW_H / bh);
        float w = bw * s, h = bh * s;
        dst.set((VIEW_W - w) / 2f, (VIEW_H - h) / 2f,
                (VIEW_W + w) / 2f, (VIEW_H + h) / 2f);
        c.drawBitmap(bg, null, dst, paint);
    }

    /**
     * Draws the level's placed objects. Tiles are decor sprites looked up through
     * the DECORS_ATLAS table; the rest are the prefabs the original instantiates
     * for each entity id (exit, hydrant, teleporter, switch, hidden object).
     */
    private void drawEntities(Canvas c, GameWorld g) {
        for (LevelData.Entity e : g.level.entities) {
            if (!e.mode) continue;
            float x = e.worldX(), y = e.worldY();
            switch (e.type) {
                case LevelData.T_TILE:
                case LevelData.T_HURT:
                    drawTile(c, g, e, x, y);
                    break;
                case LevelData.T_EXIT:
                    drawSprite(c, "AtlasFireman", "Exit1", x - camX, v(y), 60f, 80f);
                    break;
                case LevelData.T_HYDRANT:
                    drawSprite(c, "AtlasFireman", "Fire_Hydrant_ld", x - camX, v(y), 50f, 60f);
                    break;
                case LevelData.T_TELEPORT:
                    drawSprite(c, "AtlasFireman", "Teleporter1", x - camX, v(y), 50f, 60f);
                    break;
                case LevelData.T_SWITCH:
                    drawSprite(c, "FireAtlas", e.enabled ? "switch_on" : "switch_off",
                            x - camX, v(y), 40f, 40f);
                    break;
                case LevelData.T_HIDDEN: {
                    int obj = e.object;
                    if (obj < 0 || obj > 4) obj = 0;
                    String atlas = "AtlasObjetCache";
                    String name = "objetCache_" + Math.min(3, Math.max(1, g.world)) + "_" + obj;
                    drawSprite(c, atlas, name, x - camX, v(y), 50f, 50f);
                    break;
                }
                case LevelData.T_COIN:
                    drawSprite(c, "AtlasCommun", "coin02", x - camX, v(y), 36f, 36f);
                    break;
                case LevelData.T_TANK:
                    drawSprite(c, "AtlasFireman", "Fire_Hydrant_ld", x - camX, v(y), 50f, 60f);
                    break;
                case LevelData.T_VICTIM:
                    drawSprite(c, "AtlasFireman", "Pnj1", x - camX, v(y), 50f, 60f);
                    break;
                case LevelData.T_BUBBLEBLAST:
                    drawSprite(c, "FireAtlas", "bubbleblast", x - camX, v(y), 50f, 50f);
                    break;
                default:
                    break; // enemies are drawn from the mob list
            }
        }
    }

    private float v(float worldY) {
        return VIEW_H - (worldY - camY);
    }

    private void drawTile(Canvas c, GameWorld g, LevelData.Entity e, float x, float y) {
        int layer = e.sprite - 1;
        String atlasName = Atlas.decorAtlas(layer, g.world);
        String sprite = "decor" + layer;
        if (!atlas.has(atlasName, sprite)) return;

        float sx = x - camX, sy = v(y);
        float half = GameWorld.TILE_SPRITE / 2f;
        if (sx + half < 0 || sx - half > VIEW_W) return;
        if (sy + half < 0 || sy - half > VIEW_H) return;

        Bitmap b = atlas.get(atlasName, sprite);
        src.set(0, 0, b.getWidth(), b.getHeight());

        boolean flipH = (e.flip & 1) != 0;
        boolean flipV = (e.flip & 2) != 0;
        boolean rot = e.angle != 0;

        if (rot || flipH || flipV) {
            m.reset();
            m.setTranslate(sx, sy);
            if (rot) m.postRotate(-e.angle);
            if (flipH || flipV) m.postScale(flipH ? -1f : 1f, flipV ? -1f : 1f);
            m.postTranslate(-sx, -sy);
            c.save();
            c.concat(m);
        }
        dst.set(sx - half, sy - half, sx + half, sy + half);
        c.drawBitmap(b, src, dst, paint);
        if (rot || flipH || flipV) c.restore();
    }

    private void drawPlatforms(Canvas c, GameWorld g) {
        for (GameWorld.Platform p : g.platforms) {
            drawSprite(c, "AtlasFireman", "Elevator1_1", p.x - camX, v(p.y),
                    GameWorld.TILE, GameWorld.TILE);
        }
    }

    /**
     * Enemies. Only the fires have 2D atlas art (Flamme01..06 in FireAtlas) and
     * an ash state (Cendre01..05); the other enemies were Spine skeletons in the
     * original, so they are drawn as labelled placeholders rather than guessed
     * artwork.
     */
    private void drawMobs(Canvas c, GameWorld g) {
        float t = g.time * 12f;
        for (GameWorld.Mob mob : g.mobs) {
            if (mob.gone && mob.ashTimer <= 0f) continue;
            float sx = mob.x - camX, sy = v(mob.y);

            if (mob.type == LevelData.T_FLAMMER || mob.type == LevelData.T_BOSS1) {
                if (mob.ashTimer > 0f) {
                    int frame = 1 + ((int) (g.time * 8f) % 5);
                    drawSprite(c, "FireAtlas", "Cendre0" + frame, sx, sy, 50f, 50f);
                } else {
                    int frame = 1 + (int) (t % 6);
                    float size = mob.type == LevelData.T_BOSS1 ? 90f : 52f;
                    drawSprite(c, "FireAtlas", "Flamme0" + frame, sx, sy, size, size);
                }
                continue;
            }
            drawPlaceholder(c, mob, sx, sy);
        }
    }

    private void drawPlaceholder(Canvas c, GameWorld.Mob mob, float sx, float sy) {
        int color;
        String label;
        switch (mob.type) {
            case LevelData.T_CRAWLER: color = 0xFF8BC34A; label = "crawler"; break;
            case LevelData.T_RUNNER:  color = 0xFFFFB300; label = "runner"; break;
            case LevelData.T_BOMBER:  color = 0xFF5D4037; label = "bomber"; break;
            case LevelData.T_THROWER: color = 0xFF7B1FA2; label = "thrower"; break;
            case LevelData.T_SPIDER:  color = 0xFF455A64; label = "spider"; break;
            default:                  color = 0xFF9E9E9E; label = "mob"; break;
        }
        mobPaint.setColor(color);
        dst.set(sx - mob.w / 2f, sy - mob.h / 2f, sx + mob.w / 2f, sy + mob.h / 2f);
        c.drawRoundRect(dst, 6f, 6f, mobPaint);
        if (labelPaint == null) {
            labelPaint = new Paint(Paint.ANTI_ALIAS_FLAG);
            labelPaint.setColor(0xFFFFFFFF);
            labelPaint.setTextSize(14f);
            labelPaint.setTextAlign(Paint.Align.CENTER);
        }
        c.drawText(label, sx, sy + 5f, labelPaint);
    }

    private final Paint mobPaint = new Paint();
    private Paint labelPaint;

    private void drawWater(Canvas c, GameWorld g) {
        for (GameWorld.Water w : g.water) {
            // Goutte forces the sprite to 22x11 and scales by 1..2.
            float sc = 1.5f;
            drawSprite(c, "AtlasDecor", "goutte", w.x - camX, v(w.y), 22f * sc, 11f * sc);
        }
    }

    private void drawPlayer(Canvas c, GameWorld g) {
        float sx = g.px - camX, sy = v(g.py);
        String sprite;
        if (g.inputShoot) sprite = "04_Shoot";
        else if (!g.onGround) sprite = "04_Jump";
        else if (g.pvx != 0f) sprite = g.facingRight ? "03_Right" : "03_Left";
        else sprite = "03_Right";
        drawSprite(c, "AtlasFireman", sprite, sx, sy, 56f, 64f);
    }

    private void drawSprite(Canvas c, String atlasName, String name,
                            float cx, float cy, float w, float h) {
        Bitmap b = atlas.get(atlasName, name);
        src.set(0, 0, b.getWidth(), b.getHeight());
        dst.set(cx - w / 2f, cy - h / 2f, cx + w / 2f, cy + h / 2f);
        c.drawBitmap(b, src, dst, paint);
    }

    private void drawHud(Canvas c, GameWorld g) {
        float pad = 16f;
        text.setTextSize(30f);
        c.drawText("World " + g.world + "  Level " + g.levelIndex, pad, pad + 30f, text);
        c.drawText("Lives " + Math.max(0, g.life), pad, pad + 66f, text);
        c.drawText("Saved " + g.victimsSaved + "   Fire out " + g.flamesOut,
                pad, pad + 102f, text);

        // water gauge
        float gw = 260f, gh = 22f;
        float gx = pad, gy = pad + 120f;
        c.drawRect(gx, gy, gx + gw, gy + gh, outline);
        c.drawRect(gx + 2, gy + 2, gx + 2 + (gw - 4) * g.reserveWater, gy + gh - 2, fill);

        if (g.status == GameWorld.WON) {
            centered(c, "LEVEL COMPLETE", VIEW_H / 2f - 40f, 64f);
            centered(c, "tap to continue", VIEW_H / 2f + 30f, 34f);
        } else if (g.status == GameWorld.GAMEOVER) {
            centered(c, "GAME OVER", VIEW_H / 2f - 40f, 64f);
            centered(c, "tap to restart", VIEW_H / 2f + 30f, 34f);
        } else if (g.status == GameWorld.DEAD) {
            centered(c, "ouch", VIEW_H / 2f, 48f);
        }
    }

    private final Paint outline = new Paint();
    private final Paint fill = new Paint();
    {
        outline.setStyle(Paint.Style.STROKE);
        outline.setStrokeWidth(3f);
        outline.setColor(0xFFFFFFFF);
        fill.setColor(0xFF4FC3F7);
    }

    private void centered(Canvas c, String s, float y, float size) {
        text.setTextSize(size);
        float w = text.measureText(s);
        c.drawText(s, (VIEW_W - w) / 2f, y, text);
    }
}
