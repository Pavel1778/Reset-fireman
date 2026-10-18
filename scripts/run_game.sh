#!/usr/bin/env bash
# The emulator has no KVM, so system_server is slow enough that ActivityManager's
# 10s process-start timeout kills the game before libunity.so (15 MB) finishes
# loading. Free CPU by disabling the heaviest Google apps, then retry the launch
# until the process survives long enough to render.
export PATH=$PATH:/workspace/project/ca472fe0d86c46cea499aaeb79a65fd8/sdk/platform-tools
PKG=com.magmamobile.game.Fireman
LOG=/tmp/run.log
: > "$LOG"

adb root >> "$LOG" 2>&1; sleep 5; adb wait-for-device

for p in com.google.android.gms com.android.vending com.google.android.apps.nexuslauncher \
         com.google.android.inputmethod.latin com.android.dialer com.google.android.googlequicksearchbox \
         com.google.android.youtube com.google.android.apps.photos; do
  adb shell pm disable-user --user 0 "$p" >> "$LOG" 2>&1
done
adb shell settings put global hide_error_dialogs 1 >> "$LOG" 2>&1

for attempt in $(seq 1 10); do
  echo "########## attempt $attempt ##########" >> "$LOG"
  adb shell am force-stop $PKG
  adb logcat -c
  adb shell am start -n $PKG/up.PluginActivity >> "$LOG" 2>&1

  # Wait up to 3 minutes for the process to appear and stay alive.
  alive=0
  for t in $(seq 1 18); do
    sleep 10
    pid=$(adb shell pidof $PKG 2>/dev/null | tr -d '\r')
    if [ -n "$pid" ]; then alive=$((alive+1)); else alive=0; fi
    [ "$alive" -ge 6 ] && break
  done
  echo "pid=$pid alive_streak=$alive" >> "$LOG"

  if [ -n "$pid" ]; then
    sleep 90
    adb shell screencap -p /sdcard/a.png
    adb pull /sdcard/a.png "/tmp/attempt_$attempt.png" >> "$LOG" 2>&1
    echo "--- crash scan ---" >> "$LOG"
    adb logcat -d 2>/dev/null | grep -E " $pid " | grep -viE "HostConnection|Choreographer|CheckinCompat" | head -15 >> "$LOG"
    adb logcat -d 2>/dev/null | grep -iE "FATAL|UnsatisfiedLink|DllNotFound" | head -10 >> "$LOG"
    break
  fi
done
echo "RUN DONE" >> "$LOG"
