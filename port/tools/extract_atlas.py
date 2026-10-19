#!/usr/bin/env python3
"""Extract NGUI atlas sprite rectangles from the recovered Unity prefabs.

Each UIAtlas prefab lists its sprites as a flat YAML sequence:

    sprites:
    - name: decor60
      outer:
        serializedVersion: 2
        x: 0
        y: 631
        width: 128
        height: 128
      inner: ...
      rotated: 0
      paddingLeft: 0
      ...

Unity texture coordinates have their origin at the bottom-left, while PNG
decoding and Android Bitmap both use top-left, so the extractor converts y to
top-down here. That way the runtime never has to think about the flip.

Output: a JSON map atlas name -> { png, width, height, sprites: {name: {...} } }.
"""

import json
import os
import re
import sys

# The atlas prefabs live in two places in the recovered tree; both are valid and
# the game references them by atlas name, so merge by name.
ATLAS_DIRS = [
    "recovered/Fireman_Unity_Project/Assets/Resources/atlas",
    "recovered/Fireman_Unity_Project/Assets/GameObject",
]

SPRITE_RE = re.compile(r"^\s*-\s*name:\s*(\S+)\s*$")
FIELD_RE = re.compile(r"^\s+(\w+):\s*(-?\d+)\s*$")


def parse_prefab(path):
    """Return (sprites, png_guid_material) from one atlas prefab."""
    lines = open(path, encoding="utf-8", errors="replace").read().splitlines()
    sprites = {}
    in_sprites = False
    cur = None
    # outer/inner blocks both repeat x/y/width/height; only "outer" is the
    # source rectangle we want, so track which block we are inside.
    block = None

    for ln in lines:
        if re.match(r"^\s*sprites:\s*$", ln):
            in_sprites = True
            continue
        if in_sprites and re.match(r"^\s*(m_|material:)", ln):
            in_sprites = False
            cur = None
            continue
        if not in_sprites:
            continue

        m = SPRITE_RE.match(ln)
        if m:
            cur = {"name": m.group(1)}
            sprites[m.group(1)] = cur
            block = None
            continue
        if cur is None:
            continue

        bm = re.match(r"^\s+(outer|inner):\s*$", ln)
        if bm:
            block = bm.group(1)
            continue
        if block == "outer":
            fm = FIELD_RE.match(ln)
            if fm:
                cur[fm.group(1)] = int(fm.group(2))
    return sprites


def find_png(name):
    for d in ["Assets/Resources/atlas", "Assets/Texture2D"]:
        p = os.path.join("recovered/Fireman_Unity_Project", d, name + ".png")
        if os.path.exists(p):
            return p
    return None


def png_size(path):
    """Read width/height from a PNG IHDR chunk without any imaging library."""
    with open(path, "rb") as f:
        head = f.read(24)
    if head[:8] != b"\x89PNG\r\n\x1a\n":
        return None
    return (int.from_bytes(head[16:20], "big"), int.from_bytes(head[20:24], "big"))


def main():
    root = sys.argv[1] if len(sys.argv) > 1 else "."
    os.chdir(root)

    atlases = {}
    for d in ATLAS_DIRS:
        if not os.path.isdir(d):
            continue
        for fn in sorted(os.listdir(d)):
            if not fn.endswith(".prefab"):
                continue
            name = fn[:-7]
            sprites = parse_prefab(os.path.join(d, fn))
            if not sprites:
                continue
            png = find_png(name)
            if not png:
                print(f"  {name}: {len(sprites)} sprites, no PNG found (skipped)")
                continue
            w, h = png_size(png)
            out = {}
            for sn, s in sprites.items():
                if "x" not in s or "width" not in s:
                    continue
                out[sn] = {
                    "x": s["x"],
                    # Unity y is bottom-up; convert to top-down for the runtime.
                    "y": h - (s["y"] + s["height"]),
                    "w": s["width"],
                    "h": s["height"],
                    "rot": s.get("rotated", 0),
                    "padL": s.get("paddingLeft", 0),
                    "padR": s.get("paddingRight", 0),
                }
            if name in atlases:
                atlases[name]["sprites"].update(out)
            else:
                atlases[name] = {
                    # Only the file name is meaningful: the APK keeps every sheet
                    # under assets/atlas, and the loader opens it from there.
                    "png": os.path.basename(png),
                    "width": w,
                    "height": h,
                    "sprites": out,
                }
            print(f"  {name:20} {len(out):4} sprites  {w}x{h}  {os.path.basename(png)}")

    total = sum(len(a["sprites"]) for a in atlases.values())
    print(f"\natlases: {len(atlases)}, sprites total: {total}")
    out_path = "port/app/assets/atlas_regions.json"
    json.dump(atlases, open(out_path, "w"), indent=1, sort_keys=True)
    print(f"wrote {out_path}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
