#!/usr/bin/env bash
# Build a maximally-compatible, correctly-aligned and modern-signed Fireman 1.0.9 APK.
set -u
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SRC="$ROOT/original/Fireman_1.0.9_ARMv7_compact.apk"
WORK="$ROOT/work"
OUT="$ROOT/artifacts"
KS="$WORK/fireman-release.keystore"
KS_PASS="${FIREMAN_KS_PASS:?set FIREMAN_KS_PASS to the keystore password}"
mkdir -p "$WORK" "$OUT" "$ROOT/logs"

echo "[1/4] zipalign -p 4 (fixes uncompressed resources.arsc 4-byte alignment)"
zipalign -p -f 4 "$SRC" "$WORK/aligned.apk" || { echo "zipalign FAILED"; exit 1; }

echo "[2/4] apksigner sign (v1 + v2 + v3)"
apksigner sign \
  --ks "$KS" --ks-pass "pass:$KS_PASS" --key-pass "pass:$KS_PASS" \
  --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
  --out "$OUT/Fireman_1.0.9_fixed.apk" "$WORK/aligned.apk" || { echo "sign FAILED"; exit 1; }

echo "[3/4] zipalign -c verification"
zipalign -c -v 4 "$OUT/Fireman_1.0.9_fixed.apk" > "$ROOT/logs/zipalign_fixed.log" 2>&1 && echo "  zipalign: OK" || echo "  zipalign: CHECK LOG"

echo "[4/4] apksigner verify"
apksigner verify --verbose --print-certs "$OUT/Fireman_1.0.9_fixed.apk" > "$ROOT/logs/apksigner_fixed.log" 2>&1 && echo "  signature: OK" || echo "  signature: CHECK LOG"

sha256sum "$OUT/Fireman_1.0.9_fixed.apk" | tee "$OUT/SHA256SUMS.txt"
ls -l "$OUT/Fireman_1.0.9_fixed.apk"
