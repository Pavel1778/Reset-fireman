#!/usr/bin/env python3
"""Rebuild the APK keeping every original entry byte-identical, replacing only
AndroidManifest.xml (patched) and dropping the old signature files.

Why: apktool re-encodes 193 res/* files (Google Play drawables). Splicing the
patched binary manifest into the pristine original APK avoids touching anything
except the one file we intend to change.
"""
import sys, zipfile, shutil, os

orig, patched_apk, out = sys.argv[1], sys.argv[2], sys.argv[3]

src = zipfile.ZipFile(orig)
patched = zipfile.ZipFile(patched_apk)
new_manifest = patched.read('AndroidManifest.xml')

if os.path.exists(out):
    os.remove(out)
dst = zipfile.ZipFile(out, 'w', zipfile.ZIP_DEFLATED)

replaced = 0
for item in src.infolist():
    name = item.filename
    if name.startswith('META-INF/'):
        continue                      # stale signature, apksigner writes new ones
    if name == 'AndroidManifest.xml':
        data = new_manifest
        replaced += 1
    else:
        data = src.read(name)
    zi = zipfile.ZipInfo(name, date_time=item.date_time)
    zi.compress_type = item.compress_type
    zi.external_attr = item.external_attr
    dst.writestr(zi, data)

dst.close()
src.close()
patched.close()
print(f'wrote {out}; manifest replaced={replaced}')
