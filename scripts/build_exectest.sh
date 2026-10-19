#!/usr/bin/env bash
# Build the ARM32 execution test APK.
#
# The firmware is 64-bit-only in userspace, so Package Manager rejects APKs with
# armeabi-v7a libraries. That says nothing about whether the kernel can exec
# AArch32 code, which is what any 32-bit container/VM depends on.
#
# This APK has no native libraries (so it always installs) and ships two tiny
# static ELFs as assets: a 32-bit one and a 64-bit control. It runs both and
# reports exit codes, plus the real /proc/config.gz lines when readable.
set -eu

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
BT="$ROOT/sdk/build-tools/34.0.0"
PLATFORM="$ROOT/sdk/platforms/android-34/android.jar"
export JAVA_HOME="${JAVA_HOME:-/tmp/jdk/jdk-17.0.20.1+1}"
SRC="$ROOT/exectest"
OUT="$ROOT/artifacts"
KS="$ROOT/work/fireman-release.keystore"
KS_PASS="${FIREMAN_KS_PASS:-fireman123}"

mkdir -p "$OUT" "$SRC/gen" "$SRC/obj"

echo "[1/6] regenerate probe ELFs into assets"
python3 "$ROOT/scripts/make_probe_elfs.py" "$SRC/assets"
# Keep the .bin extension: an .so suffix under assets/ makes aapt2 report a bogus
# "native-code" ABI, which is only cosmetic but confusing in diagnostics.
mv -f "$SRC/assets/libarmprobe32.so" "$SRC/assets/armprobe32.bin"
mv -f "$SRC/assets/libarmprobe64.so" "$SRC/assets/armprobe64.bin"

echo "[2/6] aapt2 compile resources"
"$BT/aapt2" compile --dir "$SRC/res" -o "$SRC/obj/res.zip"

echo "[3/6] aapt2 link"
"$BT/aapt2" link \
  -o "$SRC/obj/base.apk" \
  -I "$PLATFORM" \
  --manifest "$SRC/AndroidManifest.xml" \
  --java "$SRC/gen" \
  --min-sdk-version 24 --target-sdk-version 34 \
  -A "$SRC/assets" \
  "$SRC/obj/res.zip"

echo "[4/6] javac"
"$JAVA_HOME/bin/javac" -source 8 -target 8 -nowarn \
  -bootclasspath "$PLATFORM" \
  -d "$SRC/obj/classes" \
  $(find "$SRC/java" "$SRC/gen" -name '*.java')

echo "[5/6] d8"
"$BT/d8" --lib "$PLATFORM" --min-api 24 \
  --output "$SRC/obj" \
  $(find "$SRC/obj/classes" -name '*.class')

echo "[6/6] package + align + sign"
cd "$SRC/obj"
cp base.apk unsigned.apk
"$JAVA_HOME/bin/jar" uf unsigned.apk classes.dex
cd "$ROOT"

"$BT/zipalign" -p -f 4 "$SRC/obj/unsigned.apk" "$SRC/obj/aligned.apk"
"$BT/apksigner" sign \
  --ks "$KS" --ks-pass "pass:$KS_PASS" --key-pass "pass:$KS_PASS" \
  --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
  --out "$OUT/Fireman_ARM32_Exec_Test.apk" "$SRC/obj/aligned.apk"

"$BT/apksigner" verify --verbose "$OUT/Fireman_ARM32_Exec_Test.apk" | head -4
"$BT/zipalign" -c 4 "$OUT/Fireman_ARM32_Exec_Test.apk" && echo "zipalign OK"
"$BT/aapt2" dump badging "$OUT/Fireman_ARM32_Exec_Test.apk" | grep -E "^package|native-code|sdkVersion" || true
sha256sum "$OUT/Fireman_ARM32_Exec_Test.apk"
