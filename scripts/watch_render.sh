#!/usr/bin/env bash
# The game window is focused but the framebuffer is still black: Unity needs a long
# time to reach its first frame under ndk_translation + software GL. Capture a
# screenshot every 45s for 6 minutes and record the game's CPU time to prove the
# process is actually working rather than hung.
export PATH=$PATH:/workspace/project/ca472fe0d86c46cea499aaeb79a65fd8/sdk/platform-tools
PKG=com.magmamobile.game.Fireman
LOG=/tmp/watch.log
: > "$LOG"

for i in $(seq 1 8); do
  pid=$(adb shell pidof $PKG 2>/dev/null | tr -d '\r')
  cpu=$(adb shell cat /proc/$pid/stat 2>/dev/null | awk '{print "utime="$14" stime="$15}')
  focus=$(adb shell dumpsys window 2>/dev/null | grep mCurrentFocus | head -1)
  adb shell screencap -p /sdcard/w.png 2>/dev/null
  adb pull /sdcard/w.png /tmp/watch_$i.png > /dev/null 2>&1
  echo "t=$((i*45))s pid=$pid $cpu focus=$focus" >> "$LOG"
  sleep 45
done
echo "WATCH DONE" >> "$LOG"
