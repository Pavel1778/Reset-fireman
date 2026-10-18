# FULL VERSION vs COMPACT VERSION — what the user's 31 MB file really is

## Summary

The user provided a link to the "full" 31 MB APK on trashbox.ru. I downloaded it
and compared it byte-for-byte with the file already present in the workspace.

**Result: it is the exact same file.** SHA-256 of both:

```
5a9a2c4de922cdc7ffb09424487d6977e89d4773c6c2c1b861ee51216470d3df
```

So nothing was lost, and the "full version" was never missing. What differs is
only *which* APK was published to GitHub — and that was a real mistake on my
side, now fixed.

## What actually differs between the two versions

| | `Fireman_1.0.9_ARMv7_compact.apk` (in the repo) | full 31 MB APK |
|---|---|---|
| Size | 23 598 505 bytes | **31 845 923 bytes** |
| ABI directories | `armeabi-v7a` only | `armeabi-v7a` **+ `x86`** |
| `lib/x86/*` | absent | present (8.06 MB compressed) |
| Signature | FIREMAN (custom, broken v1/SHA-256) | **CERT — original Magma Mobile** |
| `zipalign -c 4` | **FAILED** (`resources.arsc` BAD-3) | **OK** |
| Entries | 586 | 589 |

The 8.25 MB size difference is **exactly** the three x86 libraries:

```
lib/x86/libmain.so    0.01 MB
lib/x86/libmono.so    1.06 MB
lib/x86/libunity.so   6.99 MB
                      -------
                      8.06 MB
```

**The `armeabi-v7a` libraries are byte-identical between the two versions**, and
all 366 `assets/` entries are byte-identical too:

```
lib/armeabi-v7a/libmain.so   full=1be9d5656b671a35 compact=1be9d5656b671a35 same=True
lib/armeabi-v7a/libmono.so   full=f49ce2ce62681ff6 compact=f49ce2ce62681ff6 same=True
lib/armeabi-v7a/libunity.so  full=389de93f392e68d4 compact=389de93f392e68d4 same=True
assets: full=366  compact=366  identical=True
```

## The critical finding: there is no arm64 in either version

```
FULL    ABI: ['armeabi-v7a', 'x86']
COMPACT ABI: ['armeabi-v7a']
arm64 in EITHER: False
```

**The full version is bigger only because it adds x86 libraries for emulators. It
does NOT contain `arm64-v8a`.** So the full version behaves identically to the
compact one on a 64-bit-only phone.

Proven by direct test on a genuine 64-bit-only Android 14 image
(`abilist32=[]`, `ro.zygote=zygote64`):

```
$ adb install work/orig_fireman.apk            # the FULL 31 MB file
Failure [INSTALL_FAILED_NO_MATCHING_ABIS:
         Failed to extract native libraries, res=-113]

$ adb install artifacts/Fireman_1.0.9_fixed_universal.apk   # full + re-signed
Failure [INSTALL_FAILED_NO_MATCHING_ABIS: ... res=-113]
```

Both fail with the same error as the compact build. The full version is **not**
the fix for the user's phone.

## Where the full version DOES help

On a **32-bit-capable** device or emulator, the x86 libraries matter:

- On an **x86 Android emulator** the game can run through the *native* x86 code
  path instead of ARM translation. Verified: installed and launched
  (`up.PluginActivity` alive, 12 120 orange pixels rendered, no crash).
- On a **32-bit ARM device** both versions are equivalent, since the ARM
  libraries are identical.

## Correction to an earlier claim in RESTORATION.md

`RESTORATION.md` and the release notes said the compact APK "would not install at
all" because `resources.arsc` was misaligned and the signature was v1-only
SHA-256. **That was overstated and is corrected here.**

What I actually measured on an Android 11 (API 30) emulator that supports
`armeabi-v7a`:

| Build | `zipalign -c 4` | Install result |
|---|---|---|
| compact (repo) | FAILED (arsc BAD-3) | **Success** |
| full original (v1-only, Magma cert) | OK | **Success** |
| `Fireman_1.0.9_final.apk` | OK | **Success** |
| full with `resources.arsc` deliberately misaligned | FAILED (BAD-1) | **Success** |

I built a variant of the full APK with `resources.arsc` shifted to a 1-byte
offset, changing nothing else. It **still installed**. So the misalignment alone
does not block installation on these Android versions, and the earlier
"would not install at all" wording was wrong.

What remains true and verified:

- `zipalign -c 4` does **fail** on the compact APK — that is a real defect worth
  fixing, and it is fixed in the rebuilt artifacts.
- The original file's v1 signature uses SHA-256 and does not verify for the
  declared API 14–17 range (`apksigner` reports this). Re-signing fixes it.
- The compact APK's signature was **not** the original Magma Mobile one. The full
  file carries the genuine Magma Mobile certificate
  (`CN=Magma Mobile, OU=Game Studio, O=Magma Mobile, L=Argenteuil, ST=France, C=95100`).

I could not reproduce a definite *install-blocking* effect from either defect on
the emulators available. The one error I reproduced consistently as a blocker is
`INSTALL_FAILED_NO_MATCHING_ABIS` on 64-bit-only devices.

## ELF 16 KB page alignment (relevant for Android 15+ devices)

All six native libraries are 32-bit ELF with `p_align = 4096`:

```
lib/armeabi-v7a/libunity.so  LOAD p_align=[4096, 4096]  16KB-ok=False
lib/armeabi-v7a/libmono.so   LOAD p_align=[4096, 4096]  16KB-ok=False
lib/armeabi-v7a/libmain.so   LOAD p_align=[4096, 4096]  16KB-ok=False
lib/x86/libunity.so          LOAD p_align=[4096, 4096]  16KB-ok=False
lib/x86/libmono.so           LOAD p_align=[4096, 4096]  16KB-ok=False
lib/x86/libmain.so           LOAD p_align=[4096, 4096]  16KB-ok=False
```

Devices configured with 16 KB memory pages require `p_align >= 16384`. These
libraries are built for 4 KB pages. **This cannot be fixed without recompiling
from source**, which is not available. If a future Android release forces 16 KB
pages on all 64-bit devices, the 32-bit libraries would additionally fail to
load. It is a second, independent limitation on top of the ABI problem.

## Conclusion

1. The user's 31 MB file was already in the workspace and is byte-identical to
   what was downloaded from trashbox. Nothing was lost.
2. The full version contains **no arm64** — it adds **x86** for emulators. It is
   not a fix for a 64-bit-only phone.
3. The real reason the earlier GitHub asset was the compact build: **the GitHub
   web upload form caps files at 25 MB**, and the full APK is 31.8 MB. Publishing
   through the API (as done now) has a much higher limit.
4. The full original APK is now published as
   `Fireman_1.0.9_full_original.apk` — it is the genuine, unmodified file with
   Magma Mobile's original signature, useful for emulators and 32-bit-capable
   devices.
