export XCURSOR_SIZE=0
cd /home/cobkiosk/Desktop/Release

/usr/libexec/plasma-dbus-run-session-if-needed
while true; do
    /usr/bin/kwin_wayland --exit-with-session "wine /home/cobkiosk/Desktop/Release/Display\ test.exe" --no-lockscreen --xwayland

    EXIT_CODE=$?

    if [ $EXIT_CODE -eq 0 ]; then
        break
    fi

    sleep 1
done
