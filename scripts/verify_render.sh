#!/usr/bin/env bash
# Clean runtime check of the game on the (KVM-less, therefore slow) emulator.
# The launcher/systemui throw ANR dialogs that cover the game, so suppress them first.
export PATH=$PATH:/workspace/project/ca472fe0d86c46cea499aaeb79a65fd8/sdk/platform-tools
PKG=com.magmamobile.game.Fireman
ACT=$PKG/up.PluginActivity
LOG=/tmp/render.log
: > "$LOG"

adb shell settings put global hide_error_dialogs 1                 >> "$LOG" 2>&1
adb shell settings put global window_animation_scale 0             >> "$LOG" 2>&1
adb shell settings put global transition_animation_scale 0         >> "$LOG" 2>&1
adb shell settings put global animator_duration_scale 0            >> "$LOG" 2>&1
adb shell pm disable-user --user 0 com.google.android.apps.nexuslauncher >> "$LOG" 2>&1

adb shell am force-stop $PKG
adb logcat -c
sleep 3
adb shell am start -n $ACT >> "$LOG" 2>&1

# Unity on a software-rendered emulator needs a long, long time.
for i in 1 2 3 4 5 6; do
  sleep 60
  echo "--- t=${i}min ---" >> "$LOG"
  adb shell ps -A 2>/dev/null | grep -i fireman >> "$LOG"
  adb shell dumpsys window 2>/dev/null | grep mCurrentFocus >> "$LOG"
  adb shell input tap 1170 1170 >> "$LOG" 2>&1   # dismiss any Unity splash / loading screen
done

sleep 60
adb shell screencap -p /sdcard/r.png
adb pull /sdcard/r.png /tmp/r.png >> "$LOG" 2>&1

echo "=== crash scan ===" >> "$LOG"
adb logcat -d 2>/dev/null | grep -iE "FATAL|AndroidRuntime|UnsatisfiedLink|DllNotFound|ClassNotFound|NoSuchMethod" | head -20 >> "$LOG"
echo "=== unity/gles scan ===" >> "$LOG"
adb logcat -d 2>/dev/null | grep -iE "Unity|GLES|libunity|mono" | head -20 >> "$LOG"
echo "RENDER DONE" >> "$LOG"
