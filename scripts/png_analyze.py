#!/usr/bin/env python3
"""Analyse a PNG screenshot: colour count + fireman-palette presence."""
import sys, zlib, struct
from collections import Counter


def decode(path):
    d = open(path, 'rb').read()
    pos, idat = 8, b''
    w = h = ct = 0
    while pos + 8 <= len(d):
        ln = struct.unpack('>I', d[pos:pos + 4])[0]
        typ = d[pos + 4:pos + 8]
        if typ == b'IHDR':
            w, h, _bd, ct = struct.unpack('>IIBB', d[pos + 8:pos + 18])
        elif typ == b'IDAT':
            idat += d[pos + 8:pos + 8 + ln]
        elif typ == b'IEND':
            break
        pos += 12 + ln
    raw = zlib.decompress(idat)
    ch = {0: 1, 2: 3, 3: 1, 4: 2, 6: 4}[ct]
    stride = w * ch
    rows = len(raw) // stride
    prev = bytearray(stride)
    out = []
    i = 0
    for _ in range(rows):
        f = raw[i]; i += 1
        line = bytearray(raw[i:i + stride]); i += stride
        for x in range(stride):
            a = line[x - ch] if x >= ch else 0
            b = prev[x]
            c = prev[x - ch] if x >= ch else 0
            if f == 1:
                line[x] = (line[x] + a) & 255
            elif f == 2:
                line[x] = (line[x] + b) & 255
            elif f == 3:
                line[x] = (line[x] + (a + b) // 2) & 255
            elif f == 4:
                p = a + b - c
                pa, pb, pc = abs(p - a), abs(p - b), abs(p - c)
                pr = a if (pa <= pb and pa <= pc) else (b if pb <= pc else c)
                line[x] = (line[x] + pr) & 255
        out.append(bytes(line))
        prev = line
    return w, h, ch, out


def main(path):
    w, h, ch, rows = decode(path)
    cnt = Counter()
    for y in range(0, len(rows), 4):
        for x in range(0, w, 4):
            px = rows[y][x * ch:x * ch + 3]
            if len(px) == 3:
                cnt[tuple(px)] += 1
    total = sum(cnt.values())
    print(f'{path}: {w}x{h}, sampled={total}, distinct colours={len(cnt)}')
    for c, n in cnt.most_common(8):
        print(f'   rgb{c} x{n}')
    orange = sum(n for c, n in cnt.items() if c[0] > 180 and 100 < c[1] < 220 and c[2] < 90)
    nonblack = sum(n for c, n in cnt.items() if sum(c) > 30)
    print(f'orange-ish={orange}  non-black={nonblack} ({100*nonblack/total:.1f}%)')
    verdict = 'RENDERS' if len(cnt) > 20 and nonblack > total * 0.02 else 'BLANK/BLACK'
    print('VERDICT:', verdict)


if __name__ == '__main__':
    main(sys.argv[1])
