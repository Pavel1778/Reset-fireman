package com.fireman.port;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;

import org.json.JSONObject;

import java.io.IOException;
import java.io.InputStream;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

/**
 * Loads the original NGUI atlases and hands out individual sprites.
 *
 * The assets keep the atlas sheets exactly as shipped, plus a region table
 * (atlas_regions.json) produced from the Unity prefabs. Cropping on demand
 * keeps the APK small and avoids rewriting the artwork.
 *
 * Region y values in the table are already converted to top-down, so they can be
 * passed straight to Bitmap.createBitmap.
 */
public final class Atlas {

    public static final String[] WORLDS = {"AtlasDecor", "AtlasDecor2", "AtlasDecor3"};

    private final Map<String, Bitmap> sheets = new HashMap<>();
    private final Map<String, Bitmap> sprites = new HashMap<>();
    private final java.util.Set<String> warned = new java.util.HashSet<>();
    private Bitmap missing;

    private Atlas() {}

    public static Atlas load(Context ctx) {
        Atlas a = new Atlas();
        try (InputStream in = ctx.getAssets().open("atlas_regions.json")) {
            byte[] buf = new byte[in.available()];
            int off = 0;
            while (off < buf.length) {
                int r = in.read(buf, off, buf.length - off);
                if (r < 0) break;
                off += r;
            }
            JSONObject root = new JSONObject(new String(buf, "UTF-8"));
            for (Iterator<String> it = root.keys(); it.hasNext(); ) {
                String atlasName = it.next();
                JSONObject atlas = root.getJSONObject(atlasName);
                Bitmap sheet = a.sheet(ctx, atlas.getString("png"));
                if (sheet == null) continue;
                a.sheets.put(atlasName, sheet);
                JSONObject list = atlas.getJSONObject("sprites");
                for (Iterator<String> si = list.keys(); si.hasNext(); ) {
                    String spriteName = si.next();
                    JSONObject s = list.getJSONObject(spriteName);
                    int x = s.getInt("x"), y = s.getInt("y");
                    int w = s.getInt("w"), h = s.getInt("h");
                    if (x < 0 || y < 0 || w <= 0 || h <= 0
                            || x + w > sheet.getWidth() || y + h > sheet.getHeight()) {
                        continue; // region table disagrees with the sheet; skip it
                    }
                    try {
                        Bitmap b = Bitmap.createBitmap(sheet, x, y, w, h);
                        a.sprites.put(atlasName + "/" + spriteName, b);
                    } catch (IllegalArgumentException ignored) {
                        // a bad rectangle is not fatal, the sprite just falls back
                    }
                }
            }
        } catch (Exception e) {
            throw new RuntimeException("failed to load atlas regions", e);
        }
        return a;
    }

    /**
     * Opens the sheet from the APK's own assets. The region table records the
     * path the sheet had on the build host, so only its file name is meaningful
     * inside the APK.
     */
    private Bitmap sheet(Context ctx, String path) {
        String name = path.substring(path.lastIndexOf('/') + 1);
        try (InputStream in = ctx.getAssets().open("atlas/" + name)) {
            Bitmap b = BitmapFactory.decodeStream(in);
            if (b != null) b.setDensity(Bitmap.DENSITY_NONE);
            return b;
        } catch (IOException e) {
            return null;
        }
    }

    /** Returns the sprite, or a placeholder so drawing never crashes. */
    public Bitmap get(String atlasName, String spriteName) {
        String key = atlasName + "/" + spriteName;
        Bitmap b = sprites.get(key);
        if (b != null) return b;
        if (missing == null) {
            missing = Bitmap.createBitmap(8, 8, Bitmap.Config.ARGB_8888);
            missing.eraseColor(0xFFFF00FF);
        }
        if (warned.add(key)) {
            android.util.Log.w("FiremanAtlas", "missing sprite: " + key);
        }
        return missing;
    }

    public boolean has(String atlasName, String spriteName) {
        return sprites.containsKey(atlasName + "/" + spriteName);
    }

    /**
     * Resolves which atlas holds a decor layer, reproducing GameManager.getAtlas
     * and the DECORS_ATLAS table from the original.
     */
    public static String decorAtlas(int layer, int world) {
        int idx = layer >= 0 && layer < DECORS_ATLAS.length ? DECORS_ATLAS[layer] : 0;
        if (idx == 255) idx = world - 1;
        if (idx < 0 || idx >= WORLDS.length) idx = 0;
        return WORLDS[idx];
    }

    /** Per-layer atlas index from GameManager.DECORS_ATLAS. */
    public static final int[] DECORS_ATLAS = {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 0, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
            1, 1, 0, 0, 1, 1, 0, 0, 1, 1,
            0, 1, 0, 0, 255, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            2, 2, 2, 2, 2, 2, 2, 2, 2
    };
}
