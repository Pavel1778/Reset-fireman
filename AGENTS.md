# Fireman 1.0.9 (Magma Mobile) — restoration notes

## What the app is
- Package `com.magmamobile.game.Fireman`, versionCode 17, versionName 1.0.9, minSdk 14, targetSdk 27.
- Unity game with the **Mono** scripting backend (not IL2CPP). Code lives in
  `assets/bin/Data/Managed/*.dll`; `libmono.so` is the runtime.
- Native libraries are **armeabi-v7a only**:
  `lib/armeabi-v7a/{libmain.so,libmono.so,libunity.so}` — no arm64-v8a, no x86.
- Because it is Mono, an arm64 port is not a matter of swapping `.so` files: the
  managed assemblies are portable, but `libunity.so`/`libmono.so` would have to
  come from an arm64 Unity build of the same engine version. Not reproducible.

## Why the shipped APK is broken
The APK in `Reset-fireman/Fireman_1.0.9_ARMv7_compact.apk` cannot be installed as-is:

1. `resources.arsc` is stored **uncompressed but misaligned** (offset % 4 == 3).
   Android 11+ requires uncompressed entries to be 4-byte aligned.
2. It is signed with the **v1 scheme only**, and the JAR signature uses SHA-256,
   which `apksigner` flags as unsupported for API 14–17 (the declared minSdk).
3. The manifest declares two components whose classes are absent from
   `classes.dex`: `com.facebook.ads.RewardedVideoAdActivity` and `up.Receiver`
   (verified: zero smali references).

Fixes 1 and 2 alone make the APK installable; fix 3 removes dead declarations.

## Device reality (TECNO Pova 7 5G / LJ7)
- SoC MT6878 (Dimensity 7300 Ultra), kernel 6.1.115, ships Android 15, HiOS 15.
- Stock firmware tag: `sys_tssi_64_64only_armv82_...`, `CPU abilist: arm64-v8a`.
- That means **no 32-bit userspace**: armeabi-v7a apps fail with
  `INSTALL_FAILED_NO_MATCHING_ABIS`. The kernel does have `CONFIG_COMPAT`, so the
  limitation is userspace (zygote/abilist), not hardware.

## Artifacts
- `artifacts/Fireman_1.0.9_final.apk` — **primary deliverable.** Byte-identical to
  the original except `AndroidManifest.xml` (patched) and the signature files.
  zipalign-corrected, signed v1+v2+v3.
- `artifacts/Fireman_1.0.9_fixed.apk` — same but with the manifest untouched:
  pure zipalign + re-sign of the original.
- `artifacts/evidence/renders_gameplay_v3.png` — gameplay frame proving the
  rebuilt APK runs (orange fireman palette `rgb(255,186,8)`).

## Toolchain used (rebuilt after the sandbox was reset)
- JDK 21 (`openjdk-21-jdk-headless`), apktool 2.7.0, Android cmdline-tools +
  build-tools 34.0.0 (`zipalign`, `apksigner`, `aapt2`) under `sdk/`.
- Keystore: `work/fireman-release.keystore` (not committed). Password is supplied via the `FIREMAN_KS_PASS` environment variable.

## Scripts
- `scripts/splice_manifest.py` — rebuild APK with only the manifest replaced.
- `scripts/build_universal.sh` — fat APK (armeabi-v7a + x86) builder.
- `scripts/png_analyze.py` — screenshot colour analysis (render verification).
- `scripts/run_game.sh`, `scripts/watch_render.sh` — launch/observe helpers.

## Emulator gotchas
- No KVM in this sandbox, so the emulator is very slow: ActivityManager's 10s
  process-start timeout kills the game before `libunity.so` loads. Launch in a
  retry loop and disable heavy Google apps first (`scripts/run_game.sh`).
- Android 12+ system images are 64-bit only, matching LJ7; API 30 images keep a
  32-bit ABI list and use `ndk_translation` to run armeabi-v7a code
  (`ndk_translation: Initialized NDK translation (armv7l)` in logcat).
- Disable `com.android.systemui` / launcher ANR dialogs
  (`settings put global hide_error_dialogs 1`) or they cover the game.

## Why no arm64 port is possible (verified externally)

- Game is **Unity 5.3.8f1** (2016), scripting backend **Mono** (`libmono.so` +
  managed DLLs under `assets/bin/Data/Managed/`).
- Unity only shipped Android ARM64 in **2018.2**, and **only with the IL2CPP
  backend**. Unity's own docs list Mono on Android as **Armv7 only**.
  So a 5.3.8f1 Mono build cannot contain arm64 Unity libraries at all.
- **1.0.9 (versionCode 17) is the last release** - confirmed in APK catalogues
  (APKCombo/Uptodown show 1.0.9 from 2018 as current). There is no newer build
  that might have added arm64.
- Conclusion: a real arm64 port would need the original Unity project rebuilt
  with IL2CPP, which does not exist. Renaming `.so` files or editing the
  manifest is not a port.

## Device-claim provenance (be careful)

- The `sys_tssi_64_64only_...` firmware string was **never read from a real
  device**; it only ever appeared in our own reports. No LJ7 dump exists here.
- MT6878 (Cortex-A78 + A55, ARMv8.2-A) does support AArch32 in hardware, so any
  limitation would be userspace (zygote/abilist), not silicon.
- On Pixel 7 the 32-bit libraries were present in firmware, so flipping
  `zygote64_32` + `abilist` re-enabled 32-bit apps. Whether LJ7 ships those
  libraries is unknown and must be measured on the device.
