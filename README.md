# Fireman 1.0.9 — running a 32-bit-only game on a 64-bit-only phone

Fireman (Magma Mobile, `com.magmamobile.game.Fireman`, versionCode 17, Unity
5.3.8f1) ships `armeabi-v7a`-only native libraries. A TECNO LJ7 / Pova 7 5G
(MT6878) on Android 16 has no 32-bit Android userspace, so the original cannot
install there — the package manager finds no matching ABI.

This repository documents the diagnosis and contains **`port/`**, a pure-Java
re-implementation that runs the game's own assets with no native libraries at
all. Because it has no `lib/` directory, any ABI policy accepts it.

## Result

`port/build/Fireman_arm64_port.apk` — versionCode 18, versionName
`1.0.9-arm64.1`, 97 entries, **no `lib/` entries**. Installed and run on the
LJ7: the app starts, renders real artwork, and responds to touch.

| Check | Result |
| --- | --- |
| Installs on arm64-only firmware | yes — no native libs to reject |
| Starts without crash | yes, no `FATAL EXCEPTION` in logcat |
| Renders game art | yes, 0 missing sprites, 0 magenta placeholder pixels |
| Responds to input | yes — walking and jumping move the player |
| Simulation tests | all pass |

Evidence screenshots and logs are under `artifacts/evidence/`.

## What is ported, and what is not

The port is a **re-implementation**, not a translation of the original binary.
It reproduces the parts that make the game playable:

* level loading and collision against the original `.bytes` level files,
* the player state machine — walk, jump, jetpack, hose, taking damage,
* gravity and movement constants taken from the decompiled project
  (`gravity -9`, jump `3.0`, airborne walk ratio `0.9`),
* mobs with patrol and gravity, pickups, victims, exits, platforms, water,
* sprite rendering from the original texture atlases.

It is not a byte-for-byte reproduction of Unity's behaviour. Menu screens, the
shop, progression, and audio are not implemented; the port drops straight into
level 1 and advances on level completion. Treat it as a playable restoration,
not as a remake of every system.

## Layout

```
port/
  app/src/com/fireman/port/   Java sources (Atlas, GameWorld, LevelData,
                              MainActivity, Renderer)
  app/assets/                 level .bytes files, texture atlases, region table
  test/com/fireman/port/      simulation and asset tests
  tools/                      build.sh and the asset/level extraction scripts
recovered/                    the Unity project recovered from the APK
artifacts/evidence/           on-device screenshots and logcat captures
```

## Building

```bash
bash port/tools/build.sh
```

Needs a JDK and an Android SDK at `sdk/` with build-tools 34.0.0 and
`platforms/android-34`. The script compiles, dexes, packages, aligns and signs,
then reports the ABI list so an accidental native library is caught.

## Testing

The simulation tests run on a plain JVM — no device or emulator needed:

```bash
mkdir -p /tmp/pt
javac -nowarn -d /tmp/pt \
    port/app/src/com/fireman/port/LevelData.java \
    port/app/src/com/fireman/port/GameWorld.java \
    port/test/com/fireman/port/PortTest.java
java -cp /tmp/pt com.fireman.port.PortTest port/app/assets/levels
```

They cover landing, jump height, fire damage, level parsing, wall collision, and
the asset table. The asset checks exist because of a real bug: the region table
used to store the path each sheet had on the build host, so the loader opened a
path that is not in the APK and **every** atlas silently failed, leaving the
game running with magenta placeholder sprites. The test now fails if a sheet is
named by anything but a bare file name.

## Background

The other documents in this repository cover the original diagnosis:

* `DIAGNOSIS.md`, `SOLUTIONS.md`, `TROUBLESHOOTING.md` — why the original APK
  is rejected, and the no-root workarounds (a 32-bit VM such as VPhoneOS).
* `ARM32_PATH.md` — the LJ7 kernel *does* execute 32-bit code; it is the
  Android userspace that is missing.
* `RESTORATION.md`, `REBUILD.md` — the ARMv7 repair work and the rebuild.
