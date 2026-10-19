# Fireman 1.0.9 — the original game, repaired

Fireman (Magma Mobile, `com.magmamobile.game.Fireman`, versionCode 17, Unity
5.3.8f1, Mono) is a 2018 Android game whose native libraries are
`armeabi-v7a`-only. This repository contains the **original game**, repaired so
it installs and runs correctly on current Android, together with the diagnosis
of why it still cannot install on a phone whose firmware has no 32-bit
userspace at all.

The deliverable is the real game. It is **not** a remake: every asset, every
level, and all three native libraries are the publisher's own files.

## Deliverable

`Fireman_1.0.9_final.apk` — the original game, built from the **full** release
(the one that ships both `armeabi-v7a` and `x86` libraries, ~32 MB).
Byte-identical to it in 585 of its 587 entries. Only `AndroidManifest.xml` was
edited and the APK was re-aligned and re-signed.

| Check | Result |
| --- | --- |
| Same game as the original | yes — 585/587 zip entries byte-identical, `armeabi-v7a` + `x86` |
| Installs on 32-bit-capable Android | yes (verified on API 30) |
| Starts without crash | yes — no `FATAL EXCEPTION`, no `UnsatisfiedLinkError` |
| Renders the real game | yes — Unity GLES3 scene, ~11.9k fireman-orange pixels |
| Responds to touch | yes — the frame changes on input |
| zipalign | OK (the shipped APK fails) |
| Signature | OK on modern Android (the shipped APK fails) |

Evidence screenshots are under `artifacts/evidence_real/`.

## What was actually wrong, and what was fixed

The shipped APK has four real defects. None of them is "missing arm64".

1. **Not zip-aligned.** `resources.arsc` is stored uncompressed but lands on a
   4-byte-misaligned offset, so `zipalign -c 4` reports `Verification FAILED`.
   Fixed by re-aligning on rebuild.
2. **Signature not accepted on modern Android.** The APK is signed with a
   SHA-256 JAR signature while declaring `minSdkVersion 14`; `apksigner verify`
   reports it cannot be verified for API 14-17. Re-signed with a modern scheme.
3. **Two dangling components.** `com.facebook.ads.RewardedVideoAdActivity` and
   `up.Receiver` are declared in the manifest but the classes are **absent from
   `classes.dex`** (confirmed by scanning the dex). Removed, so the framework
   cannot trip over a missing class.
4. **Legacy cleartext traffic.** The game targets SDK 27 and talks to old
   `http://` endpoints that Android 9+ blocks by default.

## Why it still will not install on a 64-bit-only phone

This is measured, not assumed. On a TECNO LJ7 / Pova 7 5G (MT6878, Android 16):

```
Supported ABIs: [arm64-v8a]
32-bit ABIs:    []
zygote:         zygote64   abilist: arm64-v8a
32-bit runtime files: none present      <- no 32-bit linker, no 32-bit libc.so
32-bit ELF exec:      works (exit 42)   <- the kernel itself does run AArch32
```

The kernel executes 32-bit ARM code (`CONFIG_COMPAT=y`, confirmed from the
stock kernel image's embedded config). What is missing is the **32-bit Android
userspace**: there is no 32-bit `/system/bin/linker` and no 32-bit `libc.so`, so
no 32-bit shared library can be loaded and Package Manager reports
`INSTALL_FAILED_NO_MATCHING_ABIS`.

Reproduced exactly on an API 34 emulator whose ABI list is `arm64-v8a` only:
both the repaired APK and a universal (armeabi-v7a + x86) build fail with
`INSTALL_FAILED_NO_MATCHING_ABIS`, while the same APKs install and render
gameplay on an API 30 image that keeps a 32-bit ABI list.

Consequences, stated honestly:

* No edit to this APK can make it install there. The game needs 32-bit native
  libraries; the firmware provides no 32-bit loader or C library to run them.
* Unity 5.3.8f1 with the Mono backend **cannot produce arm64 libraries at all**
  (Unity only added Android arm64 in 2018.2, and only with IL2CPP). So the
  libraries cannot simply be swapped. 1.0.9 is also the last release.
* The two real options are: run the game inside a virtualization app that ships
  its own 32-bit Android userspace (e.g. VPhoneOS, VMOS Pro), or rebuild the
  game from the recovered Unity project with a modern Unity and IL2CPP.

## Repository layout

```
Fireman_1.0.9_final.apk          the repaired original game (the deliverable)
Fireman_1.0.9_ARMv7_compact.apk  the shipped original, unmodified (for comparison)
original/                        pristine inputs
recovered/                       the Unity project recovered from the APK
scripts/                         build, patch, splice and verification tooling
artifacts/evidence_real/         gameplay screenshots of the repaired original
artifacts/evidence/              diagnosis captures (ABI checks, install logs)
DIAGNOSIS.md SOLUTIONS.md ...    the written diagnosis
```

## Rebuilding the repaired APK

```bash
bash scripts/build_fixed.sh
```

It decodes the manifest, removes the two dangling components, re-encodes,
splices the manifest back into the pristine full APK (so no other entry is
touched), aligns with `zipalign -p 4` and signs with `apksigner`. Needs a JDK,
apktool and an Android SDK at `sdk/`. Override the input with `FIREMAN_SRC=` if
you want to build from a different copy.

## Verification

```bash
# alignment and signature
sdk/build-tools/34.0.0/zipalign -c -v 4 Fireman_1.0.9_final.apk
sdk/build-tools/34.0.0/apksigner verify --print-certs Fireman_1.0.9_final.apk

# prove it is still the same game
python3 - <<'PY'
import zipfile
a = zipfile.ZipFile('Fireman_1.0.9_full_original.apk')
b = zipfile.ZipFile('Fireman_1.0.9_final.apk')
common = set(a.namelist()) & set(b.namelist())
same = sum(a.read(n) == b.read(n) for n in common)
print(f'{same}/{len(common)} entries identical')
PY
```
