#!/usr/bin/env python3
"""Decode Fireman 1.0.9 level files.

The format is defined by LevelLoader.cs in the recovered Unity project. Every
record starts with col/row (int16) and a flag byte, and the flag byte's bits
decide which optional fields follow. Getting the optional-field order wrong
desynchronises the whole stream, so this decoder mirrors the C# field order
exactly and asserts that it consumes each file to the last byte.

Usage:
  dump_levels.py <levels-dir> [--json out.json] [--verbose]
"""

import json
import os
import struct
import sys

# Entity type ids, from the constants at the top of LevelLoader.cs.
TYPES = {
    0: "tile", 1: "updown", 2: "hydrant", 3: "coin", 4: "bomber",
    5: "crawler", 6: "fireman", 7: "flammer", 8: "runner", 9: "spider",
    10: "thrower", 11: "victim", 12: "exit", 13: "hurt", 14: "teleport",
    15: "trigger", 16: "pet", 17: "hidden", 18: "bubbleblast", 19: "boss1",
    20: "switch", 21: "tank", 99: "custom",
}

# Which parser each id uses, i.e. which optional fields are present.
# Derived from the r*() methods in LevelLoader.cs.
SIMPLE = {2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 21}  # col,row,flags only
CRAWLER = {5}                # + one extra int16 (a crawl distance)
DECOR = {0, 13, 99}          # + sprite index, optional angle/flip/ofs
UPDOWN = {1}                 # + range, speed, sprite, optional fields, tag, flags
HIDDEN = {17}                # + object id
TELEPORT = {14}              # + dest x, y
TRIGGER = {15}               # + event id, unused
PET = {16}                   # nothing extra
BUBBLE_BOSS = {18, 19}       # + optional tag
SWITCH = {20}                # optional angle/flip/ofs, tag, flags, optional ttl


class Reader:
    def __init__(self, data):
        self.d = data
        self.p = 0

    def byte(self):
        if self.p >= len(self.d):
            raise EOFError(f"read past end at offset {self.p}")
        v = self.d[self.p]
        self.p += 1
        return v

    def i16(self):
        if self.p + 2 > len(self.d):
            raise EOFError(f"read past end at offset {self.p}")
        v = struct.unpack_from("<h", self.d, self.p)[0]
        self.p += 2
        return v

    def i32(self):
        if self.p + 4 > len(self.d):
            raise EOFError(f"read past end at offset {self.p}")
        v = struct.unpack_from("<i", self.d, self.p)[0]
        self.p += 4
        return v

    def remaining(self):
        return len(self.d) - self.p


def b(f, mask):
    return (f & mask) == mask


def read_angle(r, f):
    return r.i16() if b(f, 1) else 0


def read_flip(r, f):
    return r.byte() if b(f, 2) else 0


def read_ofsx(r, f):
    return r.i16() if b(f, 4) else 0


def read_ofsy(r, f):
    return r.i16() if b(f, 8) else 0


def read_tag(r, f):
    return r.i16() if b(f, 0x80) else 0


def check_mode(f, gamemode):
    # ((f / 16) & 7 & GameMode) != 0
    return (((f // 16) & 7) & gamemode) != 0


def parse(data, gamemode=1):
    """Parse one level file into a list of entity dicts.

    Every .bytes file ends with a fixed 1024-byte zero trailer, so the stream to
    consume is data[:-1024]. (Confirmed on all 74 files: the trailer is exactly
    1024 bytes, and a correct parse consumes data[:-1024] exactly.)
    """
    if len(data) < 1024:
        raise ValueError(f"file too short ({len(data)} bytes) to hold the 1024-byte trailer")
    body = data[:-1024]
    r = Reader(body)
    n = r.i32()
    out = []
    for i in range(n):
        tid = r.byte()
        e = {"type": TYPES.get(tid, f"unknown_{tid}"), "id": tid, "_i": i}
        try:
            _parse_one(r, e, tid, gamemode)
        except Exception as ex:
            raise type(ex)(f"{ex} (record {i}, entity id {tid})") from None
        out.append(e)

    nbox = r.i32()
    boxes = []
    for _ in range(nbox):
        x1, y1, x2, y2 = r.i16(), r.i16(), r.i16(), r.i16()
        boxes.append({"x1": x1, "y1": y1, "x2": x2, "y2": y2})

    return out, boxes, r.p, len(body)


def _parse_one(r, e, tid, gamemode):
    """Read one entity's fields, mirroring the field order in LevelLoader.cs."""
    if tid in DECOR:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["sprite"] = r.byte()
        e["angle"] = read_angle(r, f)
        e["flip"] = read_flip(r, f)
        e["ofsx"] = read_ofsx(r, f)
        e["ofsy"] = read_ofsy(r, f)
        e["mode"] = check_mode(f, gamemode)
    elif tid in UPDOWN:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["range"] = r.i16()
        e["speed"] = r.i16()
        e["sprite"] = r.byte()
        e["angle"] = read_angle(r, f)
        e["flip"] = read_flip(r, f)
        e["ofsx"] = read_ofsx(r, f)
        e["ofsy"] = read_ofsy(r, f)
        e["tag"] = read_tag(r, f)
        f2 = r.byte()
        e["enabled"] = b(f2, 1)
        e["loop"] = b(f2, 2)
        e["mode"] = check_mode(f, gamemode)
    elif tid in HIDDEN:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["object"] = r.i16()
        e["mode"] = check_mode(f, gamemode)
    elif tid in TELEPORT:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["dx"] = r.i16()
        e["dy"] = r.i16()
        e["mode"] = check_mode(f, gamemode)
    elif tid in TRIGGER:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["event"] = r.i16()
        e["unused"] = r.i16()
        e["mode"] = check_mode(f, gamemode)
    elif tid in SWITCH:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["angle"] = read_angle(r, f)
        e["flip"] = read_flip(r, f)
        e["ofsx"] = read_ofsx(r, f)
        e["ofsy"] = read_ofsy(r, f)
        e["tag"] = read_tag(r, f)
        f2 = r.byte()
        e["enabled"] = b(f2, 1)
        e["loop"] = b(f2, 2)
        e["ttl"] = r.i32() if b(f2, 2) else 0
        e["mode"] = check_mode(f, gamemode)
    elif tid in BUBBLE_BOSS:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["tag"] = read_tag(r, f)
        e["mode"] = check_mode(f, gamemode)
    elif tid in CRAWLER:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["dist"] = r.i16()
        e["mode"] = check_mode(f, gamemode)
    elif tid in PET or tid in SIMPLE:
        e["col"] = r.i16()
        e["row"] = r.i16()
        f = r.byte()
        e["mode"] = check_mode(f, gamemode)
    else:
        raise ValueError(f"unknown entity id {tid} at offset {r.p}")


def world_coords(col, row):
    """LevelLoader.scaleX/scaleY: grid cell -> Unity world units."""
    return col * 50 - 500, 300 - row * 50


def main():
    if len(sys.argv) < 2:
        print(__doc__)
        return 2
    levels_dir = sys.argv[1]
    verbose = "--verbose" in sys.argv
    json_out = None
    if "--json" in sys.argv:
        json_out = sys.argv[sys.argv.index("--json") + 1]

    files = sorted(f for f in os.listdir(levels_dir) if f.endswith(".bytes"))
    if not files:
        print(f"no .bytes files in {levels_dir}")
        return 1

    all_levels = {}
    bad = []
    for fn in files:
        path = os.path.join(levels_dir, fn)
        data = open(path, "rb").read()
        try:
            ents, boxes, used, total = parse(data)
        except Exception as ex:
            bad.append((fn, f"{type(ex).__name__}: {ex}"))
            continue
        if used != total:
            bad.append((fn, f"consumed {used} of {total} bytes"))
            continue
        kinds = {}
        for e in ents:
            kinds[e["type"]] = kinds.get(e["type"], 0) + 1
        all_levels[fn] = {
            "entities": ents,
            "boxes": boxes,
            "kinds": kinds,
        }
        if verbose:
            summary = ", ".join(f"{k}={v}" for k, v in sorted(kinds.items()))
            print(f"{fn:26} ents={len(ents):5} boxes={len(boxes):3}  {summary}")

    print()
    print(f"parsed OK: {len(all_levels)}/{len(files)}")
    if bad:
        print(f"FAILED: {len(bad)}")
        for fn, why in bad:
            print(f"  {fn}: {why}")
    if json_out:
        json.dump(all_levels, open(json_out, "w"), indent=1)
        print(f"wrote {json_out} ({os.path.getsize(json_out):,} bytes)")
    return 1 if bad else 0


if __name__ == "__main__":
    sys.exit(main())
