package com.fireman.port;

import android.app.Activity;
import android.content.SharedPreferences;
import android.graphics.Canvas;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.Window;
import android.view.WindowManager;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;

/**
 * The playable port.
 *
 * This app is intentionally pure Java with no native libraries, which is what
 * lets it install on the arm64-only target device: the original APK shipped
 * armeabi-v7a libmono/libunity, and Android will not run those when the device
 * has no 32-bit userspace.
 *
 * Levels are read from the original .bytes files in assets/levels, so all 72
 * playable levels are intact and use the original layout data.
 */
public class MainActivity extends Activity implements SurfaceHolder.Callback {

    private SurfaceView surface;
    private Renderer renderer;
    private Atlas atlas;

    private GameWorld game;
    private Thread loop;
    private volatile boolean running;

    private int worldIndex = 1;
    private int levelIndex = 1;
    private int lastUnlocked = 1;

    private final List<String> levelFiles = new ArrayList<>();

    @Override
    protected void onCreate(Bundle b) {
        super.onCreate(b);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
                WindowManager.LayoutParams.FLAG_FULLSCREEN);
        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        atlas = Atlas.load(this);
        renderer = new Renderer(this, atlas);

        SharedPreferences prefs = getPreferences(MODE_PRIVATE);
        worldIndex = prefs.getInt("world", 1);
        levelIndex = prefs.getInt("level", 1);
        lastUnlocked = prefs.getInt("unlocked", 1);

        loadLevelList();

        surface = new SurfaceView(this);
        surface.getHolder().addCallback(this);
        surface.setOnTouchListener(new Touch());
        setContentView(surface);
    }

    private void loadLevelList() {
        try {
            String[] all = getAssets().list("levels");
            if (all != null) {
                for (String f : all) {
                    if (f.startsWith("level_") && f.endsWith(".bytes")) levelFiles.add(f);
                }
            }
        } catch (IOException ignored) {
        }
        java.util.Collections.sort(levelFiles);
    }

    private byte[] readAsset(String path) throws IOException {
        try (InputStream in = getAssets().open(path)) {
            byte[] buf = new byte[in.available()];
            int off = 0;
            while (off < buf.length) {
                int r = in.read(buf, off, buf.length - off);
                if (r < 0) break;
                off += r;
            }
            return buf;
        }
    }

    /** Loads the level into a fresh world, or reports why it could not. */
    private String loadLevel(int world, int level) {
        String name = String.format("level_%d_%02d.bytes", world, level);
        try {
            byte[] data = readAsset("levels/" + name);
            LevelData parsed = LevelData.parse(data);
            worldIndex = world;
            levelIndex = level;
            game = new GameWorld(parsed, world, level);
            renderer.setBackground("Bg_" + bgName(world) + ".png");
            getPreferences(MODE_PRIVATE).edit()
                    .putInt("world", world).putInt("level", level).apply();
            return null;
        } catch (Exception e) {
            return e.getClass().getSimpleName() + ": " + e.getMessage();
        }
    }

    private static String bgName(int world) {
        switch (world) {
            case 2: return "Jungle";
            case 3: return "Mountain";
            default: return "Desert";
        }
    }

    @Override
    public void surfaceCreated(SurfaceHolder holder) {
        if (game == null) {
            loadLevel(worldIndex, levelIndex);
        }
        renderer.configure(holder);
        running = true;
        // An explicit Runnable avoids java.lang.invoke.LambdaMetafactory, which
        // android.jar does not expose.
        loop = new Thread(new Runnable() {
            @Override
            public void run() {
                gameLoop();
            }
        }, "fireman-loop");
        loop.start();
    }

    @Override
    public void surfaceChanged(SurfaceHolder holder, int fmt, int w, int h) {
        renderer.configure(holder);
    }

    @Override
    public void surfaceDestroyed(SurfaceHolder holder) {
        running = false;
        try {
            if (loop != null) loop.join(500);
        } catch (InterruptedException ignored) {
        }
    }

    private void gameLoop() {
        long prev = System.nanoTime();
        while (running) {
            long now = System.nanoTime();
            float dt = (now - prev) / 1_000_000_000f;
            prev = now;
            if (dt > 0.1f) dt = 0.1f;

            GameWorld w = game;
            if (w != null) {
                w.update(dt);
                float minX = Float.MAX_VALUE, maxX = -Float.MAX_VALUE;
                for (LevelData.Entity e : w.level.entities) {
                    if (!e.mode) continue;
                    float x = e.worldX();
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                }
                renderer.updateCamera(w, minX - 100f, maxX + 100f);
            }

            Canvas c = null;
            try {
                c = surface.getHolder().lockCanvas();
                if (c != null && w != null) renderer.draw(c, w);
            } catch (Exception ignored) {
            } finally {
                if (c != null) {
                    try {
                        surface.getHolder().unlockCanvasAndPost(c);
                    } catch (Exception ignored) {
                    }
                }
            }

            try {
                Thread.sleep(16);
            } catch (InterruptedException e) {
                return;
            }
        }
    }

    /** Left third: dpad. Right third: jump and shoot. */
    private final class Touch implements View.OnTouchListener {
        private boolean leftDown, rightDown, jumpDown, shootDown;

        @Override
        public boolean onTouch(View v, MotionEvent e) {
            GameWorld w = game;
            if (w == null) return true;

            float x = e.getX() / v.getWidth();
            float y = e.getY() / v.getHeight();

            if (e.getActionMasked() == MotionEvent.ACTION_DOWN && w.status == GameWorld.WON) {
                advance();
                return true;
            }
            if (e.getActionMasked() == MotionEvent.ACTION_DOWN && w.status == GameWorld.GAMEOVER) {
                w.restartLevel();
                return true;
            }

            boolean left = false, right = false, jump = false, shoot = false;
            int count = e.getPointerCount();
            for (int i = 0; i < count; i++) {
                float px = e.getX(i) / v.getWidth();
                float py = e.getY(i) / v.getHeight();
                if (py > 0.55f) {
                    if (px < 0.2f) left = true;
                    else if (px < 0.4f) right = true;
                } else {
                    if (px < 0.75f) shoot = true;
                    else jump = true;
                }
            }
            // The final finger of a gesture must not keep steering. Without this
            // the last position of a swipe stays latched and the player walks
            // until they hit a wall.
            int action = e.getActionMasked();
            if (action == MotionEvent.ACTION_UP || action == MotionEvent.ACTION_CANCEL) {
                left = right = jump = shoot = false;
            }
            w.inputLeft = left;
            w.inputRight = right;
            w.inputJump = jump;
            w.inputShoot = shoot;
            return true;
        }
    }

    private void advance() {
        int nextLevel = levelIndex + 1;
        int nextWorld = worldIndex;
        if (nextLevel > 24) { nextLevel = 1; nextWorld++; }
        if (nextWorld > 3) { nextWorld = 1; nextLevel = 1; }
        int flat = nextWorld * 100 + nextLevel;
        if (flat > lastUnlocked) {
            lastUnlocked = flat;
            getPreferences(MODE_PRIVATE).edit().putInt("unlocked", flat).apply();
        }
        loadLevel(nextWorld, nextLevel);
    }
}
