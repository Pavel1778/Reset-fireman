#!/usr/bin/env bash
# Reproduce the LJ7 blocker: install the final APK on a 64-bit-only system image
# (abilist32 empty, ro.zygote=zygote64), which is exactly the LJ7 configuration.
# The emulator runs without KVM, so the package service comes and goes; retry.
export PATH=$PATH:/workspace/project/ca472fe0d86c46cea499aaeb79a65fd8/sdk/platform-tools
D=emulator-5556
LOG=/tmp/only64.log
: > "$LOG"

echo "abilist=$(adb -s $D shell getprop ro.product.cpu.abilist | tr -d '\r')" >> "$LOG"
echo "abilist32=[$(adb -s $D shell getprop ro.product.cpu.abilist32 | tr -d '\r')]" >> "$LOG"
echo "zygote=$(adb -s $D shell getprop ro.zygote | tr -d '\r')" >> "$LOG"

adb -s $D push /workspace/project/ca472fe0d86c46cea499aaeb79a65fd8/artifacts/Fireman_1.0.9_final.apk /data/local/tmp/f.apk >> "$LOG" 2>&1

for i in $(seq 1 20); do
  echo "--- attempt $i ---" >> "$LOG"
  out=$(timeout 300 adb -s $D shell pm install -r /data/local/tmp/f.apk 2>&1)
  echo "$out" >> "$LOG"
  case "$out" in
    *Success*) echo "RESULT: INSTALLED" >> "$LOG"; break ;;
    *NO_MATCHING_ABIS*) echo "RESULT: BLOCKED_NO_MATCHING_ABIS" >> "$LOG"; break ;;
  esac
  sleep 20
done
echo "ONLY64 DONE" >> "$LOG"
