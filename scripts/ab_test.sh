#!/usr/bin/env bash
# A/B install test on the emulator.
#   A = repo APK exactly as shipped (v1-only SHA-256 signature, resources.arsc misaligned)
#   B = our fixed APK (zipalign-corrected, v1+v2+v3 signature)
# Emulator runs without KVM, so installs are slow; everything is logged.
export PATH=$PATH:/workspace/project/ca472fe0d86c46cea499aaeb79a65fd8/sdk/platform-tools
cd /workspace/project/ca472fe0d86c46cea499aaeb79a65fd8 || exit 1
LOG=/tmp/ab_test.log
: > "$LOG"

run() {
  local label="$1" apk="$2"
  echo "=================== $label ===================" >> "$LOG"
  echo "apk: $apk" >> "$LOG"
  adb push "$apk" /data/local/tmp/t.apk >> "$LOG" 2>&1
  echo "--- pm install ---" >> "$LOG"
  timeout 900 adb shell pm install -r -d /data/local/tmp/t.apk >> "$LOG" 2>&1
  echo "--- pm exit: $? ---" >> "$LOG"
  adb shell dumpsys package com.magmamobile.game.Fireman 2>/dev/null | grep -E "versionName|codePath|signatures" | head -4 >> "$LOG"
  echo >> "$LOG"
}

run "TEST A: repo APK (v1-only SHA-256, misaligned resources.arsc)" "Reset-fireman/Fireman_1.0.9_ARMv7_compact.apk"
adb uninstall com.magmamobile.game.Fireman >> "$LOG" 2>&1
run "TEST B: fixed v3 (zipaligned, v1+v2+v3)" "artifacts/Fireman_1.0.9_fixed_v3.apk"

echo "ALL DONE" >> "$LOG"
