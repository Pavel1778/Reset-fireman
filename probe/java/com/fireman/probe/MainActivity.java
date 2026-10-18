package com.fireman.probe;

import android.app.Activity;
import android.os.Build;
import android.os.Bundle;
import android.widget.TextView;

public class MainActivity extends Activity {
    @Override
    protected void onCreate(Bundle b) {
        super.onCreate(b);
        TextView t = new TextView(this);
        t.setPadding(40, 40, 40, 40);
        t.setTextSize(16);
        t.setText("INSTALL PROBE: OK\n\n"
            + "This app has NO native libraries, no permissions,\n"
            + "and targets SDK 34. It cannot fail an ABI check.\n\n"
            + "If you can read this, your phone DOES allow\n"
            + "installing APK files from unknown sources.\n\n"
            + "Android " + Build.VERSION.RELEASE + " (API " + Build.VERSION.SDK_INT + ")\n"
            + "Device: " + Build.MANUFACTURER + " " + Build.MODEL + "\n");
        setContentView(t);
    }
}
