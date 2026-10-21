# Audit report — Fireman 1.0.9 on modern Android

This is the full audit of the repository, the artifacts and the published
release. Every statement below was produced by running the artifact; the commands
and their raw output are quoted where it matters.

The headline finding is uncomfortable but it is the truth: **the full original
APK was already installable, launchable and playable**, and an earlier revision
of this work claimed to repair defects that either were not there or that a
previous bad artifact had introduced. That revision is corrected here.

## 1. Deliverable

`Fireman_1.0.9_final.apk` — 32,090,949 bytes,
SHA-256 `e8656604513405b9d4d60e8bc17208e894cc32425d98c530fff0180e7676de21`.

It is the **pristine full original APK, re-signed**. No content is modified:

```
orig entries: 589 | new: 589
NEW vs ORIG content: 586/587 identical
  differing: ['META-INF/MANIFEST.MF']
```

`META-INF/MANIFEST.MF` necessarily differs: `apksigner` rewrites it when it
signs. `META-INF/CERT.RSA` and `META-INF/CERT.SF` (the old Magma Mobile v1
signature) are replaced by the new signature files. `AndroidManifest.xml` is
byte-identical to the original. `native-code` is still `armeabi-v7a` + `x86`.

## 2. What was actually wrong

| Claim in the earlier revision | Verdict | Evidence |
| --- | --- | --- |
| "Not zip-aligned" | **false for the full original** | `zipalign -c 4` passes on `Fireman_1.0.9_full_original.apk` |
| "Signature not accepted on modern Android" | **true, but narrow** | original is v1-only; `apksigner` cannot verify it for API 14-17; it still installs on API 30 and API 34 |
| "Two dangling components" | **true but harmless** | classes absent from `classes.dex`; Android does not refuse to install or launch for this |
| "Legacy cleartext traffic" | **irrelevant** | app targets SDK 27; Android blocks cleartext by default only from targetSdk 28 |

So exactly one change is justified: re-signing with a modern scheme
(v1 + v2 + v3). That is all `Fireman_1.0.9_final.apk` does.

### 2.1 Regression that the earlier revision introduced

The earlier `Fireman_1.0.9_final.apk` (SHA-256 `c5410786…`) decoded the manifest
with `apktool`, deleted the two components and added `usesCleartextTraffic`, then
re-encoded. The `apktool` round-trip rewrote two values:

```
ORIGINAL:  android:value="640341620909"   (Raw: "640341620909")
EARLIER:   android:value=" 640341620909"  (Raw: " 640341620909")   <- leading space
```

Those are `com.google.android.gms.games.APP_ID` and
`com.google.android.gms.appstate.APP_ID`. A leading space in a Play Games app id
breaks the sign-in/leaderboard path. The regression is gone in the current build,
because the current build never touches the manifest.

## 3. Functional testing

### 3.1 The deliverable runs (Android 11 / API 30, 32-bit ABI present)

Emulator ABI list: `x86_64,x86,arm64-v8a,armeabi-v7a,armeabi`.

```
adb install -r -t --no-streaming Fireman_1.0.9_final.apk
  -> Success

pidof com.magmamobile.game.Fireman
  -> 5929, still alive after 120 s

screencap (5 frames)
  -> 2340x1080 landscape
     distinct colours 51 864 - 64 689 per frame
     fireman-orange pixels 134 111 - 136 550 per frame

touch (tap + swipe), 5 frames
  -> frame-to-frame delta 6 867 - 17 593 changed pixels
     => the scene responds to input

logcat
  -> 0 matches for FATAL EXCEPTION / UnsatisfiedLinkError
```

Screenshots: `artifacts/evidence_real/final_v2_gameplay.png` and
`final_v2_gameplay2.png`.

### 3.2 The original behaves identically

The untouched `Fireman_1.0.9_full_original.apk` was run through the same
procedure on the same emulator: it installs, launches `up.PluginActivity`, stays
alive, and renders the same scene (134 045 and 134 682 fireman-orange pixels).
See `artifacts/evidence_real/original_untouched_gameplay.png`.

This is the A/B result that matters: **the repaired build and the original are
functionally indistinguishable**, which is exactly what a re-sign-only
deliverable should be.

### 3.3 The real blocker is the 32-bit ABI

Reproduced on an Android 14 image whose `abilist32` is empty — a faithful model
of the user's 64-bit-only firmware:

```
ro.product.cpu.abilist   = x86_64,arm64-v8a
ro.product.cpu.abilist32 = []                <- empty
ro.build.version.sdk     = 34

adb install Fireman_1.0.9_full_original.apk
  -> Failure [INSTALL_FAILED_NO_MATCHING_ABIS: ... res=-113]

adb install Fireman_1.0.9_final.apk
  -> Failure [INSTALL_FAILED_NO_MATCHING_ABIS: ... res=-113]
```

The repaired APK and the untouched original fail **identically**. No manifest
edit, no re-alignment and no re-signing changes that, because the firmware has no
32-bit `linker` and no 32-bit `libc.so` to load the game's `armeabi-v7a`
libraries. Diagnostic APKs with no native libraries (`Fireman_Install_Probe.apk`,
`Fireman_ABI_Check.apk`) install fine on the same image, which isolates the cause
to the ABI and not to the APK's packaging or signature.

### 3.4 Not tested

Full gameplay (finishing levels, the shop, the ad paths). The emulators run
without KVM and are far too slow for that.

## 4. Provenance corrections

* `Fireman_1.0.9_ARMv7_compact.apk` (23,598,505 bytes) is **not** the pristine
  original. It is `armeabi-v7a`-only, fails `zipalign -c 4`, and is signed with a
  certificate whose DN is `FIREMAN`, not `CN=Magma Mobile`. Its three
  `armeabi-v7a` libraries are byte-identical to the full original's, so it is the
  same game code, but the packaging is not the publisher's. `FULL_VS_COMPACT.md`
  already recorded that this file is a locally produced artifact; the repository
  layout in `README.md` now says so too.
* The full original is genuinely pristine: `CN=Magma Mobile, OU=Game Studio`,
  SHA-256 cert digest `01fc5e5d…`, and it is the input the deliverable is built
  from.

## 5. The path to arm64

The only route to running this on the user's 64-bit-only phone natively is an
arm64 build, and that requires recompiling from source. The Unity project was
recovered from the APK with AssetRipper and is genuine:

```
299 .cs files (~45 436 lines), 6 .unity scenes, 70 prefabs, 50 .ogg,
68 .png, 32 shaders, 4 .dll, NGUI intact
```

A Mono arm64 target does not exist (Unity added Android arm64 only in 2018.2,
IL2CPP only), so the rebuild must move to a modern Unity with IL2CPP. The
blocker is Unity licensing/activation, not the code. See `REBUILD.md`.

## 6. Repository state

Files changed by this audit:

* `README.md` — deliverable description, defect list and provenance corrected.
* `scripts/build_fixed.sh` — rewritten: re-sign only, no `apktool`.
* `SHA256SUMS`, `SHA256SUMS.txt` — updated to the new deliverable hash.
* `artifacts/evidence_real/*` — added gameplay captures for the corrected build
  and for the untouched original.

The release `v1.0.9-android-fix.1` was updated in place: the
`Fireman_1.0.9_final.apk` asset and `SHA256SUMS.txt` were replaced and the
description rewritten. The uploaded APK was re-downloaded and its hash checked
against the local build:

```
downloaded sha256: e8656604513405b9d4d60e8bc17208e894cc32425d98c530fff0180e7676de21
local      sha256: e8656604513405b9d4d60e8bc17208e894cc32425d98c530fff0180e7676de21
```

## 7. How to verify everything in this report

```bash
# 1. the deliverable is the original game, re-signed
python3 - <<'PY'
import zipfile
a = zipfile.ZipFile('artifacts/Fireman_1.0.9_full_original.apk')
b = zipfile.ZipFile('artifacts/Fireman_1.0.9_final.apk')
common = set(a.namelist()) & set(b.namelist())
same = sum(a.read(n) == b.read(n) for n in common)
print(f'{same}/{len(common)} identical; differing:',
      sorted(n for n in common if a.read(n) != b.read(n)))
PY

# 2. alignment and signature
sdk/build-tools/34.0.0/zipalign -c 4 artifacts/Fireman_1.0.9_final.apk
sdk/build-tools/34.0.0/apksigner verify --verbose artifacts/Fireman_1.0.9_final.apk

# 3. the Play Games app ids are intact
sdk/build-tools/34.0.0/aapt dump xmltree artifacts/Fireman_1.0.9_final.apk \
  AndroidManifest.xml | grep -A1 'gms.games.APP_ID'

# 4. rebuild from scratch, byte-identical result
bash scripts/build_fixed.sh
sha256sum artifacts/Fireman_1.0.9_final.apk
```
