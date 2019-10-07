adb root
adb remount
adb shell "setenforce 0"
adb shell "chmod 777 /sys/class/leds/red/brightness"
adb shell "chmod 777 /sys/class/leds/green/brightness"
adb shell "chmod 777 /sys/class/leds/blue/brightness"
adb shell "chmod 777 /sys/class/leds/lcd-backlight/brightness"
adb shell "ls -l /sys/class/leds/red/brightness"
adb shell "ls -l /sys/class/leds/green/brightness"
adb shell "ls -l /sys/class/leds/blue/brightness"
adb shell "ls -l /sys/class/leds/lcd-backlight/brightness"