# How to actually play Fireman 1.0.9 — and why the APK was rejected

## Short version

The APK is correct. Your phone has **no 32-bit Android userspace**, so it offers
no 32-bit ABI and no change to the APK can fix that. Two things are needed:

1. **Find out for sure** — install `Fireman_ABI_Check.apk` (in this release). It
   installs on any phone and tells you in one line whether 32-bit apps are
   possible on your device.
2. **If they are not** — use a **VPhoneOS** virtual machine. It is a free app that
   runs a small Android 7.1.2 system *inside* your phone and **does** support
   32-bit (`armeabi-v7a`) apps, even on 64-bit-only devices. This is the only
   no-root method that is known to work.

### It is the userspace, not the kernel

Worth stating precisely, because it is what makes step 2 work. On a TECNO LJ7 the
**kernel executes 32-bit code fine** — `CONFIG_COMPAT=y`, confirmed by running a
32-bit binary on the device itself (exit code 42, see `ARM32_PATH.md` and
`LJ7_exec_test_output.txt`). What is missing is the 32-bit *Android runtime*: no
32-bit `linker`, no 32-bit `libc.so`, `ro.zygote` is `zygote64`. Package Manager
therefore reports no 32-bit ABI and refuses the game's `armeabi-v7a` libraries.

That distinction is the whole reason a VM helps: a virtual machine brings its own
32-bit Android, and the shared kernel underneath runs it natively, with no CPU
emulation. A phone whose *kernel* refused 32-bit code would not be helped by any
VM.

Details for both steps are below.

## Step 1 — Diagnose your phone

Install **`Fireman_ABI_Check.apk`** from this release and open it. It has no
native code, so it installs everywhere, including 64-bit-only phones.

The app prints a verdict at the top:

```
========================================
  RESULT: 32-BIT APPS CAN BE INSTALLED
========================================
```
or
```
========================================
  RESULT: 32-BIT APPS ARE *NOT* SUPPORTED
========================================
```

It also shows `SUPPORTED_32_BIT_ABIS`, `ro.zygote`, `ro.product.cpu.abilist32`
and whether `/system/lib` still contains 32-bit libraries.

If you prefer the command line, with a PC and USB debugging:

```
adb shell getprop ro.product.cpu.abilist32
adb shell getprop ro.zygote
```

- `armeabi-v7a,armeabi` + `zygote64_32` → 32-bit apps work. Install
  `Fireman_1.0.9_final.apk` directly.
- empty + `zygote64` → **64-bit only**. The Fireman APK can never install here.
  Go to Step 2.

Do not trust generic "CPU info" apps for this. They show the *processor*, which is
almost always 64-bit-capable, not the *Android ABI list*, which is what decides
whether an APK can be installed. `Fireman_ABI_Check.apk` reads the correct value.

## Step 2 — If your phone is 64-bit only

### Option A — VPhoneOS (recommended: free, no root, proven)

**VPhoneOS** (formerly **VPhoneGaGa**) is a virtual machine app. It boots a real
Android 7.1.2 system inside a container on your phone, and that virtual system
**includes a 32-bit userspace**, so 32-bit apps run inside it.

It advertises 32-bit app support directly, and it is the one mainstream VM with
reports of running `armeabi-v7a` apps on a 64-bit-only host (including a
demonstration on a Pixel 8 Pro). Reports about the alternatives are mixed, so
start here.

Steps:

1. Install VPhoneOS from its official site <https://vphoneos.com/> or from Google
   Play ("VPhoneOS: Android Emulator").
2. Create a new virtual machine. Choose **Android 7.1.2** and architecture
   **"64-bit & 32-bit mixed"** (not 64-bit only).
3. Start it and let it download its ROM. If it stalls, reboot the phone and retry.
4. Grant the permissions it asks for, and disable "child process restrictions" in
   Developer options if it prompts you.
5. Inside the VM, import `Fireman_1.0.9_final.apk` (the app can import an APK from
   your storage) and install it.

Expect it to be **slower than a native app**, since it runs a whole second Android
system, but the 32-bit code itself is **not** CPU-emulated: the guest shares your
kernel, and that kernel executes AArch32 natively (proved on this device — see
`ARM32_PATH.md`). The overhead is the container and the extra system, not
instruction translation.

Note: the Play Store build of VPhoneOS may not include the root/Magisk options,
but those are **not needed** to run a 32-bit app. Do not use "modded" builds from
random sites.

### Option B — Use a device that still supports 32-bit

Simplest and fastest if you have one:

- Any phone whose `SUPPORTED_32_BIT_ABIS` is not empty (check with the diagnostic
  app). Older phones, many budget models and most devices up to ~2023 qualify.
- An Android emulator on a PC (Android 11 / API 30 or lower). Verified working:
  the game installs and renders gameplay.
- A cloud Android phone service (LDCloud, Redfinger, etc.). Some offer 32-bit
  capable instances, but this costs money and runs in a datacenter.

### Option C — Phones that ship a 32-bit translator

Some manufacturers ship a 32-bit→64-bit translator, so 32-bit apps work without
root:

- **Xiaomi devices on a China ROM** — built-in translator.
- **OnePlus 12 and newer** — a translator called **Tango** (by Amanieu Systems).

This depends entirely on the manufacturer and ROM; it is not something you can add
yourself. If your Tecno does not have it, this option does not apply.

### Option D — Root + Magisk Zygote64_32 (only works if the libraries are present)

On some 64-bit-only devices the 32-bit libraries are physically still in the
firmware; only the Android properties disable them. On those, setting

```
ro.zygote=zygote64_32
ro.vendor.product.cpu.abilist=arm64-v8a,armeabi-v7a,armeabi
ro.vendor.product.cpu.abilist32=armeabi-v7a,armeabi
```

re-enables 32-bit apps. This is what **Magisk Zygote64_32** does on the Pixel 7.

Two hard conditions:

1. **The 32-bit libraries must actually exist in your firmware.** Check with the
   diagnostic app: if `/system/lib/libc.so` is missing, this cannot work. On the
   Pixel 7 the libraries were present; on many newer devices they are simply gone.
2. **It requires an unlocked bootloader, root, and a full data wipe.** The Pixel 7
   helper is Pixel-specific; applying it to a Tecno is experimental and may brick
   the device.

This is the risky path. Do not start here. Check `/system/lib/libc.so` first —
if it is absent, skip this option entirely.

### Option E — Custom ROM with a 32-bit userspace

Building/flashing a LineageOS or AOSP image for your exact device that uses a
32-bit-capable configuration. This is days of work, needs device-specific
sources, and can brick the phone. Realistically not worth it for one game.

## What does NOT work (do not try these)

- **Renaming or copying `.so` files** into `lib/arm64-v8a/`. The code inside is
  32-bit ARM machine code; a 64-bit loader cannot execute it.
- **Editing the manifest** to declare `arm64-v8a`. The libraries are still 32-bit;
  the install will still fail, or the app will crash at startup.
- **Hex-editing the APK header** to change the ABI. Same reason.
- **"Universal" converters** that claim to turn a 32-bit APK into 64-bit. No such
  tool exists; only a rebuild from source can do it.

## Why there is no arm64 build

- The game is built with **Unity 5.3.8f1** (2016) using the **Mono** scripting
  backend.
- Unity only added Android ARM64 in **2018.2**, and **only for the IL2CPP**
  backend. Unity's own documentation lists Mono on Android as **Armv7 only**.
- **1.0.9 (versionCode 17) is the final release** by Magma Mobile.
- So no arm64 version of this game exists anywhere, and producing one would need
  the original Unity project rebuilt with IL2CPP. The project is not public.

## Summary table

| Your phone | What to do |
|---|---|
| `SUPPORTED_32_BIT_ABIS` not empty | Install `Fireman_1.0.9_final.apk` directly |
| 64-bit only, no root, want it to work | **VPhoneOS**, Android 7.1.2, mixed 32/64 VM |
| 64-bit only, another old phone available | Use that phone |
| Xiaomi CN ROM / OnePlus 12+ | The built-in translator may run it |
| 64-bit only, `/system/lib/libc.so` exists, willing to root | Magisk Zygote64_32 (risky) |
| 64-bit only, `/system/lib/libc.so` missing | **Not possible without a VM** |
