#!/usr/bin/env bash
# Build a "fat" (armeabi-v7a + x86) APK from the pristine upstream original,
# properly aligned and signed for modern Android.
set -u
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SRC="$ROOT/work/orig_fireman.apk"
WORK="$ROOT/work"; OUT="$ROOT/artifacts"
KS="$WORK/fireman-release.keystore"; KS_PASS="${FIREMAN_KS_PASS:?set FIREMAN_KS_PASS to the keystore password}"
[ -f "$SRC" ] || { echo "missing $SRC"; exit 1; }
zipalign -p -f 4 "$SRC" "$WORK/aligned_uni.apk" || exit 1
apksigner sign --ks "$KS" --ks-pass "pass:$KS_PASS" --key-pass "pass:$KS_PASS" \
  --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
  --out "$OUT/Fireman_1.0.9_fixed_universal.apk" "$WORK/aligned_uni.apk" || exit 1
zipalign -c -v 4 "$OUT/Fireman_1.0.9_fixed_universal.apk" >/dev/null 2>&1 && echo "zipalign OK" || echo "zipalign CHECK"
apksigner verify --verbose "$OUT/Fireman_1.0.9_fixed_universal.apk" | head -5
sha256sum "$OUT/Fireman_1.0.9_fixed_universal.apk"
