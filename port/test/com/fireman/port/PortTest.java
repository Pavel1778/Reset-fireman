package com.fireman.port;

import java.io.File;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.List;

/**
 * Plain-JVM harness for the port. It exercises the real parser and the real
 * simulation (no mocks, no Android types) so failures surface before the APK is
 * built.
 *
 * Checks:
 *   1. every level file parses and consumes exactly its non-trailer bytes
 *   2. every level has a player spawn, an exit, and at least one collider
 *   3. the player falls and lands on solid ground instead of through it
 *   4. a jump peaks near the expected 3.6 tiles
 *   5. water kills fire, and fire kills the player
 *   6. a level can actually be completed by reaching the exit
 */
public final class PortTest {

    private static int failures = 0;

    public static void main(String[] args) throws Exception {
        File levels = new File(args.length > 0 ? args[0] : "port/app/assets/levels");
        if (!levels.isDirectory()) {
            System.out.println("levels dir not found: " + levels.getAbsolutePath());
            System.exit(2);
        }

        List<File> files = new ArrayList<>();
        File[] all = levels.listFiles();
        if (all != null) {
            for (File f : all) if (f.getName().endsWith(".bytes")) files.add(f);
        }
        files.sort((a, b) -> a.getName().compareTo(b.getName()));

        System.out.println();
        System.out.println("== parsing " + files.size() + " level files ==");
        int ok = 0;
        for (File f : files) {
            try {
                LevelData lv = LevelData.parse(Files.readAllBytes(f.toPath()));
                check(!lv.entities.isEmpty(), f.getName() + " has entities");
                ok++;
            } catch (Exception e) {
                fail(f.getName() + " failed to parse: " + e);
            }
        }
        System.out.println("   parsed OK: " + ok + "/" + files.size());
        check(ok == files.size(), "all levels parsed");

        System.out.println();
        System.out.println("== assets ==");
        testAssetPaths(levels.getParentFile());

        System.out.println();
        System.out.println("== structural sanity ==");
        for (File f : files) {
            // pipo/testLevel are developer scratch levels in the original
            // project; they legitimately have no player or exit.
            if (f.getName().startsWith("pipo") || f.getName().startsWith("testLevel")) continue;
            LevelData lv = LevelData.parse(Files.readAllBytes(f.toPath()));
            boolean player = false, exit = false;
            for (LevelData.Entity e : lv.entities) {
                if (e.type == LevelData.T_FIREMAN) player = true;
                if (e.type == LevelData.T_EXIT) exit = true;
            }
            check(player, f.getName() + " has a player spawn");
            check(exit, f.getName() + " has an exit");
            check(!lv.boxes.isEmpty(), f.getName() + " has colliders");
        }

        System.out.println();
        System.out.println("== game modes (flag byte upper bits select modes) ==");
        testGameModes(files);

        System.out.println();
        System.out.println("== simulation ==");
        LevelData sample = null;
        try {
            // 1-01 is the tutorial and has no enemies in Normal mode, so use a
            // level that actually contains hazards for the combat checks.
            sample = LevelData.parse(
                    Files.readAllBytes(new File(levels, "level_1_02.bytes").toPath()));
        } catch (Exception e) {
            fail("could not load level_1_02.bytes: " + e);
        }
        if (sample == null) {
            fail("sample level missing");
        } else {
            testFallAndLand(sample);
            testJumpHeight(sample);
            testWaterExtinguishesFire(sample);
            testFireKillsPlayer(sample);
            testWalkToExit(sample);
            testBoundedWorld(sample);
        }

        System.out.println();
        if (failures == 0) {
            System.out.println("ALL TESTS PASSED");
        } else {
            System.out.println(failures + " TEST(S) FAILED");
            System.exit(1);
        }
    }

    /**
     * The flag byte's bits 4..6 are a per-entity presence mask for the three
     * difficulty modes (Normal=1, Hardcore=2, Extreme=4), matching
     * AppConfig.GameModeEnum and VarsManager.GameMode. Normal is the default the
     * original ships with. This checks that switching modes really changes what
     * is present, and that Normal is the least populated of the three.
     */
    private static void testGameModes(List<File> files) throws Exception {
        String[] names = {"level_1_02.bytes", "level_1_10.bytes", "level_2_05.bytes"};
        int[] modes = {1, 2, 4, 7};
        int[] totals = new int[modes.length];

        for (String n : names) {
            File f = new File(files.get(0).getParentFile(), n);
            if (!f.exists()) continue;
            byte[] data = Files.readAllBytes(f.toPath());
            int[] counts = new int[modes.length];
            for (int i = 0; i < modes.length; i++) {
                LevelData.gamemode = modes[i];
                LevelData lv = LevelData.parse(data);
                int active = 0;
                for (LevelData.Entity e : lv.entities) if (e.mode) active++;
                counts[i] = active;
                totals[i] += active;
            }
            System.out.printf("   %-20s normal=%d hardcore=%d extreme=%d all=%d%n",
                    n, counts[0], counts[1], counts[2], counts[3]);
        }
        LevelData.gamemode = 1;

        check(totals[0] > 0, "Normal mode has active entities");
        check(totals[1] >= totals[0], "Hardcore has at least as much as Normal");
        check(totals[2] >= totals[0], "Extreme has at least as much as Normal");
        check(totals[3] >= totals[2], "All-modes has at least as much as Extreme");
    }

    /** The player must never escape the level's collider envelope. */
    private static void testBoundedWorld(LevelData lv) {
        GameWorld g = new GameWorld(lv, 1, 2);
        float minX = Float.MAX_VALUE, maxX = -Float.MAX_VALUE;
        for (GameWorld.Solid s : g.solids) {
            minX = Math.min(minX, s.left);
            maxX = Math.max(maxX, s.right);
        }
        // Walk hard into each wall and make sure we stop.
        g.inputRight = true;
        for (int i = 0; i < 600; i++) g.update(1f / 60f);
        check(g.px <= maxX + GameWorld.PLAYER_HALF_W + 1f, "player cannot run past the right edge");
        g.inputRight = false;
        g.inputLeft = true;
        for (int i = 0; i < 900; i++) g.update(1f / 60f);
        check(g.px >= minX - GameWorld.PLAYER_HALF_W - 1f, "player cannot run past the left edge");
        check(Float.isFinite(g.px) && Float.isFinite(g.py), "position stayed finite");
        System.out.printf("   after running to both walls: x=%.1f (level %.0f..%.0f)%n",
                g.px, minX, maxX);
    }

    private static void testFallAndLand(LevelData lv) {
        GameWorld g = new GameWorld(lv, 1, 1);
        float startY = g.py = 400f;
        for (int i = 0; i < 240; i++) g.update(1f / 60f);
        check(g.onGround, "player lands on ground");
        check(g.py < startY, "player fell downward");
        check(g.py > -1000f, "player did not fall through the world");
        System.out.printf("   landed at y=%.1f (from %.1f), onGround=%s%n", g.py, startY, g.onGround);
    }

    private static void testJumpHeight(LevelData lv) {
        GameWorld g = new GameWorld(lv, 1, 1);
        g.py = 400f;
        for (int i = 0; i < 240; i++) g.update(1f / 60f);
        check(g.onGround, "player is grounded before jump");

        float groundY = g.py;
        float peak = groundY;
        g.inputJump = true;
        for (int i = 0; i < 60; i++) {
            g.update(1f / 60f);
            if (i == 2) g.inputJump = false;
            if (g.py > peak) peak = g.py;
        }
        float rise = peak - groundY;
        System.out.printf("   jump rise %.1f px = %.2f tiles%n", rise, rise / GameWorld.TILE);
        // 3 u/s against 9 u/s^2 gives 0.5 units = 180 px, about 3.6 tiles.
        check(rise > 120f && rise < 220f, "jump rise is close to 180 px");
    }

    /**
     * Finds a fire the player can actually stand next to, then shoots it. The
     * level grid is coarse (one tile is 50 px while the player collider is
     * 40x60), so not every cell beside a fire is reachable; the test skips those
     * rather than asserting on an impossible position.
     */
    private static void testWaterExtinguishesFire(LevelData lv) {
        GameWorld g = new GameWorld(lv, 1, 2);
        GameWorld.Mob target = null;
        float standX = 0;

        for (GameWorld.Mob m : g.mobs) {
            if (m.type != LevelData.T_FLAMMER) continue;
            float candidate = m.x - 60f;
            if (spotFree(g, candidate, m.y)) {
                target = m;
                standX = candidate;
                break;
            }
        }
        check(target != null, "found fire with a reachable firing position");
        if (target == null) return;

        g.px = standX;
        g.py = target.y;
        g.facingRight = true;
        g.inputShoot = true;
        for (int i = 0; i < 40 && !target.gone; i++) g.update(1f / 60f);

        check(target.gone || target.health < 1f, "water damaged the fire");
        System.out.printf("   fire at %.0f health %.2f gone=%s%n",
                target.homeX, target.health, target.gone);
    }

    /** True when the player's collider at (x, y) clears every level collider. */
    private static boolean spotFree(GameWorld g, float x, float y) {
        float cy = y + GameWorld.COLLIDER_CY;
        for (GameWorld.Solid s : g.solids) {
            boolean hit = x + GameWorld.PLAYER_HALF_W > s.left
                    && x - GameWorld.PLAYER_HALF_W < s.right
                    && cy + GameWorld.PLAYER_HALF_H > s.bottom
                    && cy - GameWorld.PLAYER_HALF_H < s.top;
            if (hit) return false;
        }
        return true;
    }

    private static void testFireKillsPlayer(LevelData lv) {
        GameWorld g = new GameWorld(lv, 1, 2);
        GameWorld.Mob fire = null;
        for (GameWorld.Mob m : g.mobs) {
            if (m.type == LevelData.T_FLAMMER) { fire = m; break; }
        }
        check(fire != null, "level contains fire");
        if (fire == null) return;
        g.px = fire.x;
        g.py = fire.y;
        g.update(1f / 60f);
        check(g.status == GameWorld.DEAD || g.life < 3, "touching fire hurts the player");
        System.out.println("   status after touching fire: " + g.status + " lives=" + g.life);
    }

    private static void testWalkToExit(LevelData lv) {
        GameWorld g = new GameWorld(lv, 1, 1);
        float exitX = 0, exitY = 0;
        boolean found = false;
        for (LevelData.Entity e : lv.entities) {
            if (e.type == LevelData.T_EXIT && e.mode) {
                exitX = e.worldX();
                exitY = e.worldY();
                found = true;
                break;
            }
        }
        check(found, "exit is present and active");
        if (!found) return;
        g.px = exitX;
        g.py = exitY;
        g.update(1f / 60f);
        check(g.status == GameWorld.WON, "reaching the exit completes the level");
        System.out.println("   status at exit: " + g.status);
    }

    /**
     * The region table has to name each sheet by file name only, and every sheet
     * the port actually loads must sit in the APK's assets.
     *
     * This exists because the table used to record the path each sheet had on the
     * build host ("recovered/Fireman_Unity_Project/..."). The loader opened that
     * path as an asset, which silently failed for every atlas: the game ran, but
     * every sprite was the magenta "missing" placeholder.
     *
     * The table is pretty-printed with one field per line, so a line-based scan is
     * enough; the harness has no JSON library available.
     */
    private static void testAssetPaths(File assetsDir) throws Exception {
        File table = new File(assetsDir, "atlas_regions.json");
        check(table.isFile(), "atlas_regions.json exists");

        List<String> lines = Files.readAllLines(table.toPath());
        java.util.regex.Pattern pngPat =
                java.util.regex.Pattern.compile("^\\s*\"png\":\\s*\"([^\"]+)\"");
        java.util.regex.Pattern numPat =
                java.util.regex.Pattern.compile("^\\s*\"([^\"]+)\":\\s*(-?\\d+)\\s*,?\\s*$");
        java.util.regex.Pattern namePat =
                java.util.regex.Pattern.compile("^\\s*\"([^\"]+)\":\\s*\\{\\s*$");

        // "width" can appear after the "sprites" block, so sheet sizes are
        // collected first and the bounds check runs on a second pass.
        java.util.Map<String, int[]> sizes = new java.util.HashMap<>();
        String pending = null;
        for (String line : lines) {
            java.util.regex.Matcher am = namePat.matcher(line);
            if (am.find()) { pending = am.group(1); continue; }
            java.util.regex.Matcher pm2 = java.util.regex.Pattern
                    .compile("^\\s*\"(width|height)\":\\s*(-?\\d+)").matcher(line);
            if (pm2.find() && pending != null) {
                int[] wh = sizes.computeIfAbsent(pending, k -> new int[2]);
                if (pm2.group(1).equals("width")) wh[0] = Integer.parseInt(pm2.group(2));
                else wh[1] = Integer.parseInt(pm2.group(2));
            }
        }

        // Structure, with the depth each line sits at:
        //   {                                    depth 0 -> opens to 1
        //     "AtlasCommun": {                   depth 1 -> atlas, fields at 2
        //       "png": "...", "width": 1024,
        //       "sprites": {                     depth 2
        //         "Achievement1": {              depth 3 -> sprite, fields at 4
        //           "x": 852, "y": 0, "w": 128, "h": 128
        String atlas = null, sprite = null, sheetName = null;
        boolean sheetExists = false;
        int depth = 0;
        int curX = 0, curY = 0, curW = 0, curH = 0;
        int atlases = 0, sprites = 0, hostPaths = 0, missingSheets = 0, outOfBounds = 0;

        for (String line : lines) {
            String t = line.trim();
            if (t.isEmpty()) continue;

            int closes = 0;
            for (int i = 0; i < t.length(); i++) if (t.charAt(i) == '}') closes++;
            boolean opens = t.endsWith("{");

            // A sprite's fields are emitted in alphabetical order (h, padL, w, x,
            // y), so the region is only complete once its closing brace arrives.
            if (closes > 0 && !opens) {
                depth -= closes;
                if (depth == 3 && sprite != null) {
                    sprites++;
                    int[] wh = sizes.getOrDefault(atlas, new int[]{0, 0});
                    // Only judge bounds for sheets that are really present; the
                    // table also carries a few unused placeholder atlases.
                    if (sheetExists
                            && (curX < 0 || curY < 0 || curW <= 0 || curH <= 0
                            || (wh[0] > 0 && curX + curW > wh[0])
                            || (wh[1] > 0 && curY + curH > wh[1]))) {
                        fail("sprite region out of bounds in " + atlas
                                + "/" + sprite + ": x=" + curX + " y=" + curY
                                + " w=" + curW + " h=" + curH
                                + " sheet=" + wh[0] + "x" + wh[1]);
                        outOfBounds++;
                    }
                    sprite = null;
                }
                continue;
            }

            int here = depth;
            if (opens) depth++;

            java.util.regex.Matcher am = namePat.matcher(line);
            if (am.find()) {
                String name = am.group(1);
                if (here == 1) {
                    atlas = name;
                    atlases++;
                    sheetExists = false;
                } else if (here == 3) {
                    sprite = name;
                    curX = curY = curW = curH = 0;
                }
                continue;
            }

            java.util.regex.Matcher pm = pngPat.matcher(line);
            if (pm.find() && here == 2) {
                String png = pm.group(1);
                // A '/' means the table is still storing a build-host path, which
                // the loader cannot open from the APK.
                if (png.indexOf('/') >= 0) {
                    fail("sheet path is not a bare file name in " + atlas + ": " + png);
                    hostPaths++;
                }
                sheetName = png.substring(png.lastIndexOf('/') + 1);
                sheetExists = new File(new File(assetsDir, "atlas"), sheetName).isFile();
                if (!sheetExists && LOADED_ATLASES.contains(atlas)) {
                    fail("sheet for " + atlas + " missing from assets: " + sheetName);
                    missingSheets++;
                }
                continue;
            }

            java.util.regex.Matcher nm = numPat.matcher(line);
            if (nm.find() && here == 4) {
                String key = nm.group(1);
                int val = Integer.parseInt(nm.group(2));
                if (key.equals("x")) curX = val;
                else if (key.equals("y")) curY = val;
                else if (key.equals("w")) curW = val;
                else if (key.equals("h")) curH = val;
            }
        }

        check(hostPaths == 0, "every sheet is named by file name, not build-host path");
        check(missingSheets == 0, "every loaded atlas sheet is present in assets");
        check(outOfBounds == 0, "every sprite region fits inside its sheet");
        System.out.println("   atlases: " + atlases + "   sprite regions: " + sprites);
    }

    /** The atlases the port actually opens; see Atlas.WORLDS and Renderer. */
    private static final List<String> LOADED_ATLASES = java.util.Arrays.asList(
            "AtlasCommun", "AtlasDecor", "AtlasDecor2", "AtlasDecor3",
            "AtlasFireman", "AtlasObjetCache", "FireAtlas");

    private static void check(boolean cond, String what) {
        if (cond) {
            System.out.println("   ok   " + what);
        } else {
            fail(what);
        }
    }

    private static void fail(String what) {
        failures++;
        System.out.println("   FAIL " + what);
    }
}
