#!/usr/bin/env bash
# Install the final deliverable APK and confirm it reaches the gameplay frame.
export PATH=$PATH:/workspace/project/ca472fe0d86c46cea499aaeb79a65fd8/sdk/platform-tools
cd /workspace/project/ca472fe0d86c46cea499aaeb79a65fd8 || exit 1
PKG=com.magmamobile.game.Fireman
LOG=/tmp/final_test.log
: > "$LOG"

adb root >> "$LOG" 2>&1; sleep 5; adb wait-for-device
adb shell settings put global hide_error_dialogs 1 >> "$LOG" 2>&1
adb uninstall $PKG >> "$LOG" 2>&1

adb push artifacts/Fireman_1.0.9_final.apk /data/local/tmp/f.apk >> "$LOG" 2>&1
timeout 900 adb shell pm install -r /data/local/tmp/f.apk >> "$LOG" 2>&1
echo "--- install done ---" >> "$LOG"
adb shell dumpsys package $PKG 2>/dev/null | grep -E "versionName|signatures" | head -3 >> "$LOG"

adb logcat -c
adb shell am start -n $PKG/up.PluginActivity >> "$LOG" 2>&1
sleep 120

# Unity splash appears after a while, then the gameplay frame.
for i in $(seq 1 8); do
  adb shell screencap -p /sdcard/f.png 2>/dev/null
  adb pull /sdcard/f.png /tmp/final_$i.png > /dev/null 2>&1
  pid=$(adb shell pidof $PKG 2>/dev/null | tr -d '\r')
  echo "t=$((i*60))s pid=$pid" >> "$LOG"
  sleep 60
done

adb logcat -d 2>/dev/null | grep -iE "FATAL|UnsatisfiedLink|DllNotFound|AndroidRuntime" | head -10 >> "$LOG"
echo "FINAL TEST DONE" >> "$LOG"
