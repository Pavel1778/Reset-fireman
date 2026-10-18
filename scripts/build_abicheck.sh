#!/usr/bin/env bash
# Build the Fireman ABI Check diagnostic APK.
# Deliberately has NO native libraries, so it installs on any phone including
# 64-bit-only devices. It reports the device's real 32-bit ABI support.
set -eu
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
BT="$ROOT/sdk/build-tools/34.0.0"
PLATFORM="$ROOT/sdk/platforms/android-34/android.jar"
export JAVA_HOME=/usr/lib/jvm/java-21-openjdk-amd64
SRC="$ROOT/abicheck"
OUT="$ROOT/artifacts"
mkdir -p "$OUT" "$SRC/gen" "$SRC/obj"

echo "[1/5] aapt2 compile resources"
"$BT/aapt2" compile --dir "$SRC/res" -o "$SRC/obj/res.zip"

echo "[2/5] aapt2 link"
"$BT/aapt2" link \
  -o "$SRC/obj/base.apk" \
  -I "$PLATFORM" \
  --manifest "$SRC/AndroidManifest.xml" \
  --java "$SRC/gen" \
  --min-sdk-version 21 --target-sdk-version 34 \
  "$SRC/obj/res.zip"

echo "[3/5] javac"
"$JAVA_HOME/bin/javac" -source 8 -target 8 -nowarn \
  -bootclasspath "$PLATFORM" \
  -d "$SRC/obj/classes" \
  $(find "$SRC/java" "$SRC/gen" -name '*.java')

echo "[4/5] d8"
"$BT/d8" --lib "$PLATFORM" --min-api 21 \
  --output "$SRC/obj" \
  $(find "$SRC/obj/classes" -name '*.class')

echo "[5/5] package + align + sign"
cd "$SRC/obj"
cp base.apk unsigned.apk
"$JAVA_HOME/bin/jar" uf unsigned.apk classes.dex 2>/dev/null || {
  # jar may not support 'uf' with add; fall back to zip
  "$BT/../build-tools/34.0.0/aapt2" version >/dev/null 2>&1 || true
}
cd "$ROOT"
"$BT/zipalign" -p -f 4 "$SRC/obj/unsigned.apk" "$SRC/obj/aligned.apk"
: "${FIREMAN_KS_PASS:?set FIREMAN_KS_PASS to the keystore password}"
"$BT/apksigner" sign \
  --ks "$ROOT/work/fireman-release.keystore" \
  --ks-pass "pass:$FIREMAN_KS_PASS" --key-pass "pass:$FIREMAN_KS_PASS" \
  --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
  --out "$OUT/Fireman_ABI_Check.apk" "$SRC/obj/aligned.apk"

"$BT/apksigner" verify --verbose "$OUT/Fireman_ABI_Check.apk" | head -4
"$BT/zipalign" -c 4 "$OUT/Fireman_ABI_Check.apk" && echo "zipalign OK"
sha256sum "$OUT/Fireman_ABI_Check.apk"
