#!/usr/bin/env python3
"""Measure the opaque bounding box of atlas sprites, in pure Python.

Why this exists: the level grid places tiles 50 local units apart, but atlas
sprites are typically padded to power-of-two cells (e.g. 128x128). Before
deciding whether the port should draw a tile at its native sprite size or
stretch it to one grid cell, we need to know how much of each sprite is real
art versus transparent padding.

PNG decoding is done with zlib + the standard unfiltering algorithm so no
third-party imaging library is required.
"""

import json
import os
import struct
import sys
import zlib


def read_png(path):
    """Return (width, height, rgba_bytes) for an 8-bit RGBA/RGB PNG."""
    data = open(path, "rb").read()
    if data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError("not a PNG")

    pos = 8
    idat = b""
    width = height = bitdepth = colortype = None
    while pos < len(data):
        (length,) = struct.unpack_from(">I", data, pos)
        ctype = data[pos + 4:pos + 8]
        chunk = data[pos + 8:pos + 8 + length]
        pos += 12 + length
        if ctype == b"IHDR":
            width, height, bitdepth, colortype = struct.unpack_from(">IIBB", chunk, 0)
        elif ctype == b"IDAT":
            idat += chunk
        elif ctype == b"IEND":
            break

    if bitdepth != 8 or colortype not in (2, 6):
        raise ValueError(f"unsupported PNG: depth={bitdepth} colortype={colortype}")

    channels = 3 if colortype == 2 else 4
    raw = zlib.decompress(idat)
    stride = width * channels
    out = bytearray(height * stride)
    prev = bytearray(stride)

    p = 0
    for y in range(height):
        ftype = raw[p]
        p += 1
        line = bytearray(raw[p:p + stride])
        p += stride
        # Undo the per-scanline filter (PNG spec section 9.2).
        if ftype == 1:
            for i in range(channels, stride):
                line[i] = (line[i] + line[i - channels]) & 0xFF
        elif ftype == 2:
            for i in range(stride):
                line[i] = (line[i] + prev[i]) & 0xFF
        elif ftype == 3:
            for i in range(stride):
                a = line[i - channels] if i >= channels else 0
                line[i] = (line[i] + ((a + prev[i]) >> 1)) & 0xFF
        elif ftype == 4:
            for i in range(stride):
                a = line[i - channels] if i >= channels else 0
                b = prev[i]
                c = prev[i - channels] if i >= channels else 0
                pa, pb, pc = abs(b - c), abs(a - c), abs(a + b - 2 * c)
                pr = a if (pa <= pb and pa <= pc) else (b if pb <= pc else c)
                line[i] = (line[i] + pr) & 0xFF
        out[y * stride:(y + 1) * stride] = line
        prev = line

    return width, height, channels, bytes(out)


def alpha_bbox(img, rx, ry, rw, rh, threshold=8):
    """Bounding box of pixels with alpha > threshold inside a region."""
    w, h, ch, px = img
    if ch != 4:
        return (rx, ry, rw, rh)  # no alpha channel: whole region is opaque
    # Seed with the region's own bounds so the comparison below stays absolute.
    minx, miny, maxx, maxy = rx + rw, ry + rh, -1, -1
    for y in range(ry, ry + rh):
        row = y * w * 4
        for x in range(rx, rx + rw):
            if px[row + x * 4 + 3] > threshold:
                if x < minx: minx = x
                if x > maxx: maxx = x
                if y < miny: miny = y
                if y > maxy: maxy = y
    if maxx < 0:
        return None
    return (minx - rx, miny - ry, maxx - minx + 1, maxy - miny + 1)


def main():
    root = sys.argv[1] if len(sys.argv) > 1 else "."
    os.chdir(root)
    regions = json.load(open("port/app/assets/atlas_regions.json"))

    report = {}
    for atlas in sorted(regions):
        info = regions[atlas]
        path = info["png"]
        if not os.path.exists(path):
            continue
        img = read_png(path)
        content = {}
        for name, s in sorted(info["sprites"].items()):
            bb = alpha_bbox(img, s["x"], s["y"], s["w"], s["h"])
            if bb is None:
                continue
            content[name] = {"bb": list(bb), "region": [s["w"], s["h"]]}
        report[atlas] = content

    json.dump(report, open("port/sprite_bounds.json", "w"), indent=1, sort_keys=True)

    # Summarise how tightly the art fills its padded cell for the tile atlas.
    for atlas in ("AtlasDecor", "AtlasDecor2", "AtlasDecor3", "AtlasDecorSD"):
        if atlas not in report:
            continue
        ratios = []
        for name, c in report[atlas].items():
            if not name.startswith("decor"):
                continue
            rw, rh = c["region"]
            bw, bh = c["bb"][2], c["bb"][3]
            if rw and rh:
                ratios.append(max(bw / rw, bh / rh))
        if ratios:
            ratios.sort()
            print(f"{atlas:14} decor sprites={len(ratios):4} "
                  f"fill-ratio min={ratios[0]:.3f} median={ratios[len(ratios)//2]:.3f} max={ratios[-1]:.3f}")
    print("wrote port/sprite_bounds.json")
    return 0


if __name__ == "__main__":
    sys.exit(main())
