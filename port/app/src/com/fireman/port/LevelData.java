package com.fireman.port;

import java.util.ArrayList;
import java.util.List;

/**
 * Parser for Fireman level files, mirroring LevelLoader.cs from the original
 * Unity build.
 *
 * Every record starts with col/row (int16) and a flag byte. The flag's low bits
 * decide which optional fields follow, so the field order below has to match the
 * C# reader exactly or the whole stream desynchronises. Entity ids come from the
 * switch in loadLevel().
 *
 * The files end with a fixed 1024-byte zero trailer, which is not part of the
 * stream; the parser consumes everything before it.
 *
 * This class deliberately has no Android dependency so it can be exercised by a
 * plain JVM test.
 */
public final class LevelData {

    public static final int T_TILE = 0, T_UPDOWN = 1, T_HYDRANT = 2, T_COIN = 3,
            T_BOMBER = 4, T_CRAWLER = 5, T_FIREMAN = 6, T_FLAMMER = 7, T_RUNNER = 8,
            T_SPIDER = 9, T_THROWER = 10, T_VICTIM = 11, T_EXIT = 12, T_HURT = 13,
            T_TELEPORT = 14, T_TRIGGER = 15, T_PET = 16, T_HIDDEN = 17,
            T_BUBBLEBLAST = 18, T_BOSS1 = 19, T_SWITCH = 20, T_TANK = 21;

    /** Every level file ends with this many zero bytes that are not game data. */
    public static final int TRAILER = 1024;

    /** One placed object. Unused fields stay zero, as in the C# structs. */
    public static final class Entity {
        public int type;
        public int index;      // record order; drives decor draw depth
        public int col, row;   // grid cell
        public int sprite;     // 1-based; decor sprite is "decor" + (sprite-1)
        public int angle, flip, ofsx, ofsy;
        public int range, speed, tag, dist, object, dx, dy, event, ttl;
        public boolean enabled, loop, mode;

        public int worldX() { return scaleX(col) + ofsx; }
        public int worldY() { return scaleY(row) - ofsy; }
    }

    /** A solid rectangle. These are the level's real colliders. */
    public static final class Box {
        public int x1, y1, x2, y2;

        public int left()   { return x1 * 50 - 525; }
        public int right()  { return x2 * 50 - 525; }
        public int top()    { return 300 - y1 * 50 + 25; }
        public int bottom() { return 300 - y2 * 50 + 25; }
    }

    public final List<Entity> entities = new ArrayList<>();
    public final List<Box> boxes = new ArrayList<>();

    /** LevelLoader.scaleX: grid column to UI-space x. */
    public static int scaleX(int col) { return col * 50 - 500; }

    /** LevelLoader.scaleY: grid row to UI-space y. */
    public static int scaleY(int row) { return 300 - row * 50; }

    private final byte[] d;
    private int p;

    private LevelData(byte[] body) { this.d = body; }

    public static LevelData parse(byte[] fileBytes) {
        if (fileBytes.length < TRAILER) {
            throw new IllegalArgumentException("level file too short: " + fileBytes.length);
        }
        byte[] body = new byte[fileBytes.length - TRAILER];
        System.arraycopy(fileBytes, 0, body, 0, body.length);
        LevelData lv = new LevelData(body);
        lv.read();
        return lv;
    }

    private void read() {
        int n = i32();
        for (int i = 0; i < n; i++) {
            int type = u8();
            Entity e = new Entity();
            e.type = type;
            e.index = i;
            switch (type) {
                case T_TILE:
                case T_HURT:
                    decor(e);
                    break;
                case T_UPDOWN:
                    e.col = i16(); e.row = i16();
                    int f = u8();
                    e.range = i16();
                    e.speed = i16();
                    e.sprite = u8();
                    e.angle = angle(f); e.flip = flip(f); e.ofsx = ofsx(f); e.ofsy = ofsy(f);
                    e.tag = tag(f);
                    int f2 = u8();
                    e.enabled = (f2 & 1) == 1;
                    e.loop = (f2 & 2) == 2;
                    e.mode = mode(f);
                    break;
                case T_CRAWLER:
                    e.col = i16(); e.row = i16();
                    int fc = u8();
                    e.dist = i16();
                    e.mode = mode(fc);
                    break;
                case T_HIDDEN:
                    e.col = i16(); e.row = i16();
                    int fh = u8();
                    e.object = i16();
                    e.mode = mode(fh);
                    break;
                case T_TELEPORT:
                    e.col = i16(); e.row = i16();
                    int ft = u8();
                    e.dx = i16(); e.dy = i16();
                    e.mode = mode(ft);
                    break;
                case T_TRIGGER:
                    e.col = i16(); e.row = i16();
                    int fg = u8();
                    e.event = i16();
                    i16(); // unused in the original reader
                    e.mode = mode(fg);
                    break;
                case T_SWITCH:
                    e.col = i16(); e.row = i16();
                    int fs = u8();
                    e.angle = angle(fs); e.flip = flip(fs); e.ofsx = ofsx(fs); e.ofsy = ofsy(fs);
                    e.tag = tag(fs);
                    int fs2 = u8();
                    e.enabled = (fs2 & 1) == 1;
                    e.loop = (fs2 & 2) == 2;
                    e.ttl = (fs2 & 2) == 2 ? i32() : 0;
                    e.mode = mode(fs);
                    break;
                case T_BUBBLEBLAST:
                case T_BOSS1:
                    e.col = i16(); e.row = i16();
                    int fb = u8();
                    e.tag = tag(fb);
                    e.mode = mode(fb);
                    break;
                default: // hydrant, coin, bomber, fireman, flammer, runner, spider,
                         // thrower, victim, exit, pet, tank
                    e.col = i16(); e.row = i16();
                    e.mode = mode(u8());
                    break;
            }
            entities.add(e);
        }

        int nbox = i32();
        for (int i = 0; i < nbox; i++) {
            Box b = new Box();
            b.x1 = i16(); b.y1 = i16(); b.x2 = i16(); b.y2 = i16();
            boxes.add(b);
        }
        if (p != d.length) {
            throw new IllegalStateException("level parse consumed " + p + " of " + d.length + " bytes");
        }
    }

    private void decor(Entity e) {
        e.col = i16();
        e.row = i16();
        int f = u8();
        e.sprite = u8();
        e.angle = angle(f);
        e.flip = flip(f);
        e.ofsx = ofsx(f);
        e.ofsy = ofsy(f);
        e.mode = mode(f);
    }

    private int angle(int f) { return (f & 1) == 1 ? i16() : 0; }
    private int flip(int f)  { return (f & 2) == 2 ? u8() : 0; }
    private int ofsx(int f)  { return (f & 4) == 4 ? i16() : 0; }
    private int ofsy(int f)  { return (f & 8) == 8 ? i16() : 0; }
    private int tag(int f)   { return (f & 0x80) == 0x80 ? i16() : 0; }

    /** check_Mode: (((f / 16) & 7) & GameMode) != 0. */
    private static boolean mode(int f) { return (((f / 16) & 7) & GameMode()) != 0; }

    /** The original persists the selected game mode; level 1 is the default. */
    public static int gamemode = 1;
    private static int GameMode() { return gamemode; }

    private int u8() {
        if (p >= d.length) throw new IllegalStateException("read past end at " + p);
        return d[p++] & 0xFF;
    }

    private int i16() {
        if (p + 2 > d.length) throw new IllegalStateException("read past end at " + p);
        int v = (d[p] & 0xFF) | ((d[p + 1] & 0xFF) << 8);
        p += 2;
        return (short) v;
    }

    private int i32() {
        if (p + 4 > d.length) throw new IllegalStateException("read past end at " + p);
        int v = (d[p] & 0xFF) | ((d[p + 1] & 0xFF) << 8)
                | ((d[p + 2] & 0xFF) << 16) | ((d[p + 3] & 0xFF) << 24);
        p += 4;
        return v;
    }
}
