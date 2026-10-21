#!/usr/bin/env bash
# Build the deliverable: the pristine full original APK, re-signed with a modern
# signature scheme (v1 + v2 + v3).
#
# No content is modified. Every entry except META-INF/MANIFEST.MF stays
# byte-identical to the original; AndroidManifest.xml is left exactly as Magma
# Mobile shipped it. An earlier revision round-tripped the manifest through
# apktool, which corrupted the Play Games APP_ID values and served no purpose
# (see FULL_VS_COMPACT.md).
set -u
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
SRC="${FIREMAN_SRC:-$ROOT/artifacts/Fireman_1.0.9_full_original.apk}"
WORK="$ROOT/work"
OUT="$ROOT/artifacts"
KS="$WORK/fireman-release.keystore"
KS_PASS="${FIREMAN_KS_PASS:-fireman123}"
ALIGN="${FIREMAN_ZIPALIGN:-$ROOT/sdk/build-tools/34.0.0/zipalign}"
SIGNER="${FIREMAN_APKSIGNER:-$ROOT/sdk/build-tools/34.0.0/apksigner}"
mkdir -p "$WORK" "$OUT" "$ROOT/logs"

echo "[1/4] strip the old signature (keeps every other entry byte-identical)"
python3 - "$SRC" "$WORK/stripped.apk" <<'PY'
import sys, zipfile
src, dst = sys.argv[1], sys.argv[2]
zin = zipfile.ZipFile(src, 'r')
with zipfile.ZipFile(dst, 'w', zipfile.ZIP_DEFLATED) as zout:
    for it in zin.infolist():
        if it.filename.startswith('META-INF/'):
            continue
        zi = zipfile.ZipInfo(it.filename, date_time=it.date_time)
        zi.compress_type = it.compress_type
        zi.external_attr = it.external_attr
        zi.internal_attr = it.internal_attr
        zout.writestr(zi, zin.read(it.filename))
zin.close()
PY

echo "[2/4] zipalign -p 4"
"$ALIGN" -p -f 4 "$WORK/stripped.apk" "$WORK/aligned.apk" || { echo "zipalign FAILED"; exit 1; }

echo "[3/4] apksigner sign (v1 + v2 + v3)"
"$SIGNER" sign \
  --ks "$KS" --ks-pass "pass:$KS_PASS" --key-pass "pass:$KS_PASS" \
  --ks-key-alias fireman \
  --v1-signing-enabled true --v2-signing-enabled true --v3-signing-enabled true \
  --out "$OUT/Fireman_1.0.9_final.apk" "$WORK/aligned.apk" || { echo "sign FAILED"; exit 1; }

echo "[4/4] verify"
"$ALIGN" -c 4 "$OUT/Fireman_1.0.9_final.apk" && echo "  zipalign: OK"
"$SIGNER" verify --verbose --print-certs "$OUT/Fireman_1.0.9_final.apk" > "$ROOT/logs/apksigner_final.log" 2>&1 \
  && echo "  signature: OK" || echo "  signature: CHECK LOG"

python3 - "$SRC" "$OUT/Fireman_1.0.9_final.apk" <<'PY'
import sys, zipfile
a, b = zipfile.ZipFile(sys.argv[1]), zipfile.ZipFile(sys.argv[2])
common = set(a.namelist()) & set(b.namelist())
same = sum(a.read(n) == b.read(n) for n in common)
diff = sorted(n for n in common if a.read(n) != b.read(n))
print(f"  content: {same}/{len(common)} entries identical to the original")
print(f"  differing (expected: signature only): {diff}")
PY
sha256sum "$OUT/Fireman_1.0.9_final.apk"
