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

Run `scripts/install_apk_tools.sh` to restore everything. It installs into
`~/apktools/bin` (not `/opt`, which is not writable here) and adds that to
`~/.bashrc`.

- `sdk/build-tools/34.0.0/`: `aapt2`, `apksigner`, `zipalign`, `d8`, `dexdump`,
  `aapt`. Present in-repo, nothing to install.
- JDK 17 at `$JAVA_HOME` (`apksigner` silently no-ops without a JDK, which once
  made signature checks look like they had run when they had not).
- `apktool` 2.11.1 — decode and rebuild APKs.
- `jadx` 1.5.1 — decompile to Java, for checking what an APK actually does.
- Python: `lief` (ELF parsing), `capstone` (disassembly), `androguard` (dex).
  Use these instead of hand-rolled `struct` parsing when validating ELFs.
- No cross-compiler and no `qemu-user`, so target code is built byte-by-byte or
  not at all; validate with `capstone`/`lief` before shipping.
- Keystore: `work/fireman-release.keystore` (not committed). Password is supplied via the `FIREMAN_KS_PASS` environment variable.

## Scripts
- `scripts/install_apk_tools.sh` — restore apktool/jadx/lief/capstone/androguard.
- `scripts/splice_manifest.py` — rebuild APK with only the manifest replaced.
- `scripts/build_universal.sh` — fat APK (armeabi-v7a + x86) builder.
- `scripts/png_analyze.py` — screenshot colour analysis (render verification).
- `scripts/make_probe_elfs.py` — hand-build the ARM32/ARM64 exec probe ELFs
  (no cross-compiler here). Rebuilds the assets for the exec test.
- `scripts/build_exectest.sh` — build `Fireman_ARM32_Exec_Test.apk`.
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

## The kernel question is now ANSWERED (settled 2026-10)

- The stock LJ7 kernel image was fetched from `device_tecno_LJ7-kernel`
  (branch `lineage-23.2`, `Image.gz`) and its **embedded config extracted** via
  the `IKCFG_ST` marker (`CONFIG_IKCONFIG_PROC=y`). Saved as
  `evidence_kernel/lj7_kernel.config`, and uploaded to the release.
- Kernel build id in that image is `6.1.115-android14-11-g6b18f0b574ab-ab12901745`
  which **matches the LJ7 in Device Info HW**, so the config is the real one.
- Decisive options: `CONFIG_COMPAT=y`, `CONFIG_COMPAT_BINFMT_ELF=y`,
  `CONFIG_KUSER_HELPERS=y`, `CONFIG_COMPAT_VDSO=y`, `CONFIG_ARM64_4K_PAGES=y`,
  `CONFIG_ARM64_VA_BITS=39`.
- **So the kernel executes AArch32 natively.** The install failure is a
  *userspace* policy only (no 32-bit zygote / no 32-bit system libs), which is
  why Package Manager reports no matching ABI. Do not describe the device as
  unable to run 32-bit code.
- The 4K page size matches the game's libs (`p_align=4096`), so no 16K-page
  alignment problem either.
- Consequence: a virtualization app that ships its **own 32-bit Android
  userspace** can run the game natively, on the same cores, with no CPU
  emulation. VPhoneOS advertises 32-bit app support; VMOS Pro ships ARM 32-bit
  guest ROMs (e.g. LineageOS 14.1 ARM).
- On-device confirmation is `Fireman_ARM32_Exec_Test.apk` (no native libs, so it
  always installs): ships `armprobe32.bin` (ELF32/ARM, `exit_group(42)`) and
  `armprobe64.bin` (ELF64/AArch64, `exit_group(43)`) as assets, runs both, and
  prints exit codes plus `/proc/config.gz` lines. Read 42+43 as "kernel runs
  32-bit", which is what the config already says.
- The probe ELFs are hand-built by `scripts/make_probe_elfs.py` because no
  cross-compiler exists in this sandbox. Validate any edit with `readelf -a` and
  by decoding the three instructions (`mov r7,#248; mov r0,#42; svc #0` on ARM;
  `movz x0,#42; movz x8,#94; svc #0` on AArch64). `exit_group` is 248 on ARM,
  94 on AArch64; `exit` is 1 and 93.

## Two real paths, and their honest costs

- **Path A (no rebuild):** install a virtualization app with a 32-bit guest
  Android and run `Fireman_1.0.9_full_original.apk` inside it. Cheap, depends on
  the app's own limits on Android 16 (untested here).
- **Path B (rebuild):** Unity **2017.4.16 LTS** or **2018.2+**, scripting backend
  **IL2CPP**, Target Architectures **ARM64**. Unity 5.3.8f1 Mono cannot produce
  arm64, so 2017.4.16 is the minimum. The recovered project is in the release
  (`Fireman_Unity_Project_recovered.zip`). Expect work: NGUI is 130 recovered
  scripts, fonts/materials from AssetRipper need hand-fixing, and the
  third-party SDKs (Advertisements, Google Analytics) should be dropped.
- Repairing the APK (signing, alignment, manifest, dex) is a dead end for
  installation: it never adds `arm64-v8a`, and without a 32-bit linker even a
  forced install cannot load the libs.

