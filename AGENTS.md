# Fireman 1.0.9 (Magma Mobile) — restoration notes

## Current deliverable (settled)
- The deliverable is the **repaired original APK**, `Fireman_1.0.9_final.apk`:
  the publisher's own game, taken from `artifacts/Fireman_1.0.9_full_original.apk`
  and **re-signed v1+v2+v3 with no content change**. 586/587 content entries are
  byte-identical to the original; the only difference is `META-INF/MANIFEST.MF`,
  which `apksigner` rewrites. `AndroidManifest.xml` is untouched.
- SHA-256 `e8656604513405b9d4d60e8bc17208e894cc32425d98c530fff0180e7676de21`,
  32,090,949 bytes.
- A pure-Java re-implementation once lived in `port/`. It was **rejected by the
  user** ("doesn't resemble the original game, completely broken") and has been
  deleted. Do not reintroduce it. The user wants the original game fixed, not a
  remake.
- Verified gameplay: installed on an API 30 emulator (which keeps a 32-bit ABI
  list), launched, rendered a Unity GLES3 scene, ~135k fireman-orange pixels per
  frame, frames change on input, no `FATAL EXCEPTION` and no
  `UnsatisfiedLinkError`. The untouched original was A/B-tested the same way and
  behaves identically. Screenshots in `artifacts/evidence_real/`.
- Reproduced the 64-bit-only block on an API 34 emulator (`abilist32` empty):
  both the repaired APK and the untouched original fail with
  `INSTALL_FAILED_NO_MATCHING_ABIS`. The same APKs install fine on API 30.

## Do not re-introduce these (reverted mistakes)
An earlier revision round-tripped the manifest through `apktool` to delete two
components and add `usesCleartextTraffic`. That was wrong:
- It **introduced a regression**: `apktool` rewrote the Play Games `APP_ID`
  values from `640341620909` to `" 640341620909"` (leading space), breaking the
  Games sign-in path.
- Android does not refuse to install or launch an app because the manifest
  declares a class absent from `classes.dex`, so deleting the two components
  changed nothing observable.
- `usesCleartextTraffic` is moot at targetSdk 27; Android blocks cleartext by
  default only from targetSdk 28.
`scripts/build_fixed.sh` now re-signs the pristine APK and touches no content.
Keep it that way. See `AUDIT.md`.

## What the app is
- Package `com.magmamobile.game.Fireman`, versionCode 17, versionName 1.0.9, minSdk 14, targetSdk 27.
- Unity game with the **Mono** scripting backend (not IL2CPP). Code lives in
  `assets/bin/Data/Managed/*.dll`; `libmono.so` is the runtime.
- Native libraries are **armeabi-v7a only**:
  `lib/armeabi-v7a/{libmain.so,libmono.so,libunity.so}` — no arm64-v8a, no x86.
- Because it is Mono, an arm64 port is not a matter of swapping `.so` files: the
  managed assemblies are portable, but `libunity.so`/`libmono.so` would have to
  come from an arm64 Unity build of the same engine version. Not reproducible.

## What was actually wrong (measured)
The **full original** (`artifacts/Fireman_1.0.9_full_original.apk`) is in much
better shape than earlier notes claimed. It already passes `zipalign -c 4`,
already installs on API 30 and API 34, and already renders gameplay. Its one real
defect is that it is signed **v1-only with SHA-256**, which `apksigner` flags as
unsupported for the declared API 14-17. Re-signing with v1+v2+v3 fixes exactly
that.

The separate file `Fireman_1.0.9_ARMv7_compact.apk` (23,598,505 bytes) **is not
the publisher's original**. It is `armeabi-v7a`-only, fails `zipalign -c 4`, and
is signed with a certificate whose DN is `FIREMAN`, not `CN=Magma Mobile`. Its
three `armeabi-v7a` libraries are byte-identical to the full original's. Earlier
notes confused this locally produced file with the real release, which is how
"not zip-aligned" and "would not install" entered the docs. `FULL_VS_COMPACT.md`
had already recorded the correction; treat the compact file as a comparison
artifact only.

Manifest-declared components whose classes are absent from `classes.dex`
(`com.facebook.ads.RewardedVideoAdActivity`, `up.Receiver`) do **not** prevent
install or launch on modern Android. Removing them is optional and was reverted
because the `apktool` round-trip needed to do it corrupted the Play Games
`APP_ID` values.

## Device reality (TECNO Pova 7 5G / LJ7)
- SoC MT6878 (Dimensity 7300 Ultra), kernel 6.1.115, ships Android 15, HiOS 15.
- Stock firmware tag: `sys_tssi_64_64only_armv82_...`, `CPU abilist: arm64-v8a`.
- That means **no 32-bit userspace**: armeabi-v7a apps fail with
  `INSTALL_FAILED_NO_MATCHING_ABIS`. The kernel does have `CONFIG_COMPAT`, so the
  limitation is userspace (zygote/abilist), not hardware.

## Artifacts
- `artifacts/Fireman_1.0.9_final.apk` — **primary deliverable.** The pristine full
  original, re-signed v1+v2+v3. No content changed; only `META-INF/MANIFEST.MF`
  differs. Built by `scripts/build_fixed.sh`.
- `artifacts/Fireman_1.0.9_full_original.apk` — the untouched original (Magma
  Mobile certificate intact), the input to the build and the right file to use if
  you need signature compatibility with an installed Play copy.
- `artifacts/Fireman_1.0.9_universal_fixed.apk` — content-identical to the
  deliverable, produced by the same re-sign-only method; kept for continuity.
- `Fireman_1.0.9_ARMv7_compact.apk` (repo root) — v7a-only local artifact, **not**
  the publisher's release. Comparison only.
- `artifacts/evidence_real/` — gameplay screenshots of the deliverable and of the
  untouched original (A/B).

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
  `zygote64_32` + `abilist` re-enabled 32-bit apps. On the LJ7 the equivalent
  probe now shows the opposite for libraries: **no 32-bit `linker` or `libc.so`
  at all**, so there is nothing to flip. Only the kernel side is capable.

## The kernel question is now ANSWERED (settled 2026-10)

Confirmed twice over: by the kernel config, and by a real run on the LJ7.

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
  prints exit codes, the SELinux domain, and `/proc/config.gz` lines. Read 42+43
  as "kernel runs 32-bit", which is what the config already says.

### Measured on the LJ7 (2026-10), so stop treating this as inference

```
targetSdk of this app: 28
SELinux domain: u:r:untrusted_app_27:s0:c96,c258,c512,c768
32-bit (ELF32/ARM, exit 42):             exit code: 42   <- execution worked
64-bit control (ELF64/AArch64, exit 43): exit code: 43   <- execution worked
32-bit runtime files: none present
64-bit runtime files: /system/bin/linker64, /system/lib64/libc.so, ...
zygote: zygote64   abilist: arm64-v8a
```

Exit 42 is the 32-bit ELF running to completion. Had the kernel lacked AArch32,
`execve` would have failed with `ENOEXEC` and there would be no exit code at all,
so this is not a fallback or a misread. The same screen shows no 32-bit `linker`
or `libc.so` in the firmware, which is the whole userspace reason Package Manager
reports no 32-bit ABI. Do not describe the device as unable to run 32-bit code.

## targetSdk 28 is required for exec, and it is not a mistake

The first build of the exec test used targetSdk 34 and both probes failed with
`IOException: ... error=13, Permission denied` while the file was mode 0755.
That is SELinux, not a missing exec bit and not the kernel.

`seapp_contexts` assigns an app's domain from its targetSdk:

```
minTargetSdkVersion=34 -> untrusted_app
minTargetSdkVersion=29 -> untrusted_app_29
minTargetSdkVersion=28 -> untrusted_app_27
```

Only the legacy domains may exec a file in the app's own home directory:

```
untrusted_app_27.te:  allow untrusted_app_27 app_data_file:file execute_no_trans;
untrusted_app_all.te: allow untrusted_app_all app_data_file:file { r_file_perms execute };
```

`untrusted_app_29` and above get `execute` (mmap, for shared libraries) but not
`execute_no_trans`, and starting a process needs `execute_no_trans`. Hence EACCES
at targetSdk 34. Termux targets 28 for exactly this reason. Verified against the
`android16-release` branch, matching the LJ7, and `MIN_INSTALLABLE_TARGET_SDK`
is 24, so a targetSdk 28 APK still installs on Android 16.

Do not "fix" this by raising targetSdk, and do not look for a writable directory
that allows exec: `nativeLibraryDir` sits on `apk_data_file`, and no domain has
`execute_no_trans` on it either. Exec of app-owned files works only through the
legacy domain, so targetSdk 28 is the whole trick. The app prints
`/proc/self/attr/current` so the domain is visible on screen as proof.

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

