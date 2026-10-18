#!/usr/bin/env python3
"""Patch the decoded Fireman AndroidManifest.xml:

1. Remove <activity com.facebook.ads.RewardedVideoAdActivity> - declared in the
   manifest but the class is absent from classes.dex (dangling component).
2. Remove <receiver up.Receiver> - same problem; the billing notify receiver
   class was stripped, leaving a dangling declaration.
3. Allow cleartext HTTP: the game targets SDK 27 but talks to legacy
   http:// endpoints (magmamobile API, vungle, google-analytics), which
   Android 9+ blocks by default.

Both removed classes have zero references from smali, so removal is safe and
strictly reduces the chance of ActivityManager/ClassNotFound failures.
"""
import re
import sys

p = sys.argv[1] if len(sys.argv) > 1 else 'AndroidManifest.xml'
m = open(p, encoding='utf-8').read()
before = len(m)

m2 = re.sub(r'\s*<activity[^>]*android:name="com\.facebook\.ads\.RewardedVideoAdActivity"[^>]*/>', '', m)
print('Removed RewardedVideoAdActivity:', m2 != m)
m = m2

m2 = re.sub(r'\s*<receiver[^>]*android:name="up\.Receiver"[^>]*>.*?</receiver>', '', m, flags=re.S)
print('Removed up.Receiver:', m2 != m)
m = m2

if 'usesCleartextTraffic' not in m:
    m = m.replace('<application ', '<application android:usesCleartextTraffic="true" ', 1)
    print('Added usesCleartextTraffic=true')

open(p, 'w', encoding='utf-8').write(m)
print('size', before, '->', len(m))
