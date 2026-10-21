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

`Fireman_1.0.9_final.apk` — the original game, taken from the **full** release
(the one that ships both `armeabi-v7a` and `x86` libraries, ~32 MB) and re-signed
with a modern signature scheme. **No content is modified**: 586 of its 587
content entries are byte-identical to the original, and the only difference is
`META-INF/MANIFEST.MF`, which `apksigner` rewrites when it re-signs. The
`AndroidManifest.xml` is exactly as Magma Mobile shipped it.

| Check | Result |
| --- | --- |
| Same game as the original | yes — 586/587 entries byte-identical, `armeabi-v7a` + `x86` |
| Installs on 32-bit-capable Android | yes (verified on API 30) |
| Starts without crash | yes — no `FATAL EXCEPTION`, no `UnsatisfiedLinkError` |
| Renders the real game | yes — Unity GLES3 scene, ~135k fireman-orange pixels |
| Responds to touch | yes — the frame changes on input |
| zipalign | OK (already OK in the original) |
| Signature | v1 + v2 + v3 (the original is v1-only) |

Evidence screenshots are under `artifacts/evidence_real/`.

## What was actually wrong, and what was fixed

The honest answer: **almost nothing was wrong with the full original APK**, and
none of it is "missing arm64".

The full original already passes `zipalign -c 4`, already installs, already
launches and already renders gameplay on any device that still supports 32-bit
apps. The single real defect is that it is signed **v1-only** with a SHA-256 JAR
signature while declaring `minSdkVersion 14`, so `apksigner verify` reports it
cannot be verified for API 14-17. `Fireman_1.0.9_final.apk` fixes exactly that by
re-signing with v1 + v2 + v3, and touches nothing else.

An earlier revision of this repository went further: it round-tripped the
manifest through `apktool`, deleted two manifest components and added
`usesCleartextTraffic`. That was a mistake and has been reverted, because:

* The two components (`com.facebook.ads.RewardedVideoAdActivity`, `up.Receiver`)
  are absent from `classes.dex`, but Android does not fail to install or launch
  an app because the manifest declares a class the dex lacks — they are only
  touched if the (absent) ad SDK or the retired Market billing broadcasts fire.
  Removing them changed nothing that could be observed.
* `usesCleartextTraffic` is irrelevant: the app targets SDK 27, and Android only
  blocks cleartext by default from targetSdk 28 up.
* The `apktool` round-trip **introduced** a regression — it rewrote the two Play
  Games `APP_ID` meta-data values from `640341620909` to `" 640341620909"`, with
  a leading space. Re-signing the untouched APK avoids that entirely.

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
artifacts/Fireman_1.0.9_full_original.apk
                                 the pristine full original, untouched (the input)
Fireman_1.0.9_ARMv7_compact.apk  a v7a-only, 23.5 MB APK (NOT the original — see below)
original/                        pristine inputs
recovered/                       the Unity project recovered from the APK
scripts/                         build and verification tooling
artifacts/evidence_real/         gameplay screenshots of the repaired original
artifacts/evidence/              diagnosis captures (ABI checks, install logs)
DIAGNOSIS.md SOLUTIONS.md ...    the written diagnosis
```

Note on `Fireman_1.0.9_ARMv7_compact.apk`: it is 23.5 MB, contains only
`armeabi-v7a`, fails `zipalign -c 4`, and is signed with a certificate whose DN
is `FIREMAN`, **not** Magma Mobile. Its `armeabi-v7a` libraries are byte-identical
to the full original's, but the packaging is not the publisher's. It is kept only
as a comparison point; the deliverable is built from
`artifacts/Fireman_1.0.9_full_original.apk`.

## Rebuilding the repaired APK

```bash
bash scripts/build_fixed.sh
```

It strips the old signature, aligns with `zipalign -p 4` and signs with
`apksigner` (v1 + v2 + v3). It needs a JDK and an Android SDK at `sdk/`.
Override the input with `FIREMAN_SRC=` to build from a different copy.

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
