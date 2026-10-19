#!/usr/bin/env bash
# Builds the Fireman arm64-native port.
#
# The app has no native libraries on purpose: pure Java + assets means Android
# installs and runs it on an arm64-only device, where the original armeabi-v7a
# libmono/libunity cannot load.
#
# Output: port/build/Fireman_arm64_port.apk
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/../.." && pwd)"
SDK="$ROOT/sdk"
BT="$SDK/build-tools/34.0.0"
PLATFORM="$SDK/platforms/android-34/android.jar"
APP="$ROOT/port/app"
OUT="$ROOT/port/build"
KS="$ROOT/work/fireman-release.keystore"
KS_PASS="${FIREMAN_KS_PASS:-fireman123}"

rm -rf "$OUT"
mkdir -p "$OUT/classes" "$OUT/dex"

echo "== compiling java =="
find "$APP/src" -name '*.java' > "$OUT/sources.txt"
javac -source 8 -target 8 -nowarn \
    -bootclasspath "$PLATFORM" -classpath "$PLATFORM" \
    -d "$OUT/classes" @"$OUT/sources.txt"

if [ ! -d "$OUT/classes/com/fireman/port" ]; then
    echo "compile produced no classes" >&2
    exit 1
fi

echo "== dexing =="
"$BT/d8" --release --min-api 24 --lib "$PLATFORM" \
    --output "$OUT/dex" $(find "$OUT/classes" -name '*.class')

echo "== compiling resources =="
"$BT/aapt2" compile --dir "$APP/res" -o "$OUT/res.zip"

echo "== linking =="
"$BT/aapt2" link \
    -o "$OUT/unsigned.apk" \
    -I "$PLATFORM" \
    --manifest "$APP/AndroidManifest.xml" \
    --min-sdk-version 24 --target-sdk-version 34 \
    -A "$APP/assets" \
    "$OUT/res.zip"

echo "== adding classes.dex =="
python3 - "$OUT/unsigned.apk" "$OUT/dex/classes.dex" <<'PY'
import sys, zipfile
apk, dex = sys.argv[1], sys.argv[2]
with zipfile.ZipFile(apk, 'a', zipfile.ZIP_DEFLATED) as z:
    z.write(dex, 'classes.dex')
PY

echo "== zipalign =="
"$BT/zipalign" -f -p 4 "$OUT/unsigned.apk" "$OUT/aligned.apk"

echo "== signing =="
"$BT/apksigner" sign \
    --ks "$KS" --ks-pass pass:"$KS_PASS" --key-pass pass:"$KS_PASS" \
    --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
    --out "$OUT/Fireman_arm64_port.apk" "$OUT/aligned.apk"

echo "== verify =="
"$BT/apksigner" verify --print-certs "$OUT/Fireman_arm64_port.apk" | head -4
echo
"$BT/aapt2" dump badging "$OUT/Fireman_arm64_port.apk" | grep -E "^package|sdkVersion|native-code|launchable-activity|application-label" || true
echo
echo "lib/ entries in APK (must be none):"
python3 - "$OUT/Fireman_arm64_port.apk" <<'PY'
import sys, zipfile
z = zipfile.ZipFile(sys.argv[1])
libs = [n for n in z.namelist() if n.startswith('lib/') or n.endswith('.so')]
print("  ", libs if libs else "none  <-- arm64-only safe")
print("   entries:", len(z.namelist()))
PY
ls -la "$OUT/Fireman_arm64_port.apk"
