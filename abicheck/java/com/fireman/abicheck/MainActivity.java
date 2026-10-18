package com.fireman.abicheck;

import android.app.Activity;
import android.os.Build;
import android.os.Bundle;
import android.widget.ScrollView;
import android.widget.TextView;

import java.io.File;
import java.lang.reflect.Method;
import java.util.Arrays;

public class MainActivity extends Activity {

    private static String prop(String key) {
        try {
            Class<?> sp = Class.forName("android.os.SystemProperties");
            Method get = sp.getMethod("get", String.class);
            Object v = get.invoke(null, key);
            return v == null ? "(null)" : v.toString();
        } catch (Throwable t) {
            return "(unavailable)";
        }
    }

    private static String arr(String[] a) {
        if (a == null || a.length == 0) return "(EMPTY - none supported)";
        return Arrays.toString(a);
    }

    private static String dirInfo(String path) {
        File d = new File(path);
        if (!d.exists()) return path + " -> does NOT exist";
        String[] f = d.list();
        int n = f == null ? 0 : f.length;
        boolean libc = new File(d, "libc.so").exists();
        return path + " -> exists, " + n + " entries, libc.so=" + libc;
    }

    @Override
    protected void onCreate(Bundle b) {
        super.onCreate(b);
        StringBuilder s = new StringBuilder();

        String[] a32 = Build.SUPPORTED_32_BIT_ABIS;
        boolean can32 = a32 != null && a32.length > 0;

        s.append("========================================\n");
        s.append(can32 ? "  RESULT: 32-BIT APPS CAN BE INSTALLED\n"
                       : "  RESULT: 32-BIT APPS ARE *NOT* SUPPORTED\n");
        s.append("========================================\n\n");

        s.append("Device: ").append(Build.MANUFACTURER).append(" ")
                .append(Build.MODEL).append("\n");
        s.append("Android: ").append(Build.VERSION.RELEASE)
                .append("  (API ").append(Build.VERSION.SDK_INT).append(")\n");
        s.append("Build ID: ").append(Build.ID).append("\n");
        s.append("Hardware: ").append(Build.HARDWARE).append("\n\n");

        s.append("--- ABI lists (what Android allows) ---\n");
        s.append("SUPPORTED_ABIS       : ").append(arr(Build.SUPPORTED_ABIS)).append("\n");
        s.append("SUPPORTED_32_BIT_ABIS: ").append(arr(a32)).append("\n");
        s.append("SUPPORTED_64_BIT_ABIS: ").append(arr(Build.SUPPORTED_64_BIT_ABIS)).append("\n\n");

        s.append("--- System properties ---\n");
        s.append("ro.product.cpu.abi       : ").append(prop("ro.product.cpu.abi")).append("\n");
        s.append("ro.product.cpu.abilist   : ").append(prop("ro.product.cpu.abilist")).append("\n");
        s.append("ro.product.cpu.abilist32 : ").append(prop("ro.product.cpu.abilist32")).append("\n");
        s.append("ro.product.cpu.abilist64 : ").append(prop("ro.product.cpu.abilist64")).append("\n");
        s.append("ro.zygote                : ").append(prop("ro.zygote")).append("\n");
        s.append("ro.dalvik.vm.native.bridge: ").append(prop("ro.dalvik.vm.native.bridge")).append("\n");
        s.append("ro.build.product         : ").append(prop("ro.build.product")).append("\n");
        s.append("security patch           : ")
                .append(prop("ro.build.version.security_patch")).append("\n\n");

        s.append("--- 32-bit system libraries present? ---\n");
        s.append(dirInfo("/system/lib")).append("\n");
        s.append(dirInfo("/vendor/lib")).append("\n\n");

        s.append("--- What this means ---\n");
        if (can32) {
            s.append("This phone can run 32-bit (armeabi-v7a) apps.\n");
            s.append("The Fireman 1.0.9 fix should install here.\n");
            s.append("If it still failed, the cause was something else:\n");
            s.append("Play Protect, a signature conflict with an already\n");
            s.append("installed copy, or free storage.\n");
        } else {
            s.append("This phone has NO 32-bit Android userspace, so the\n");
            s.append("Fireman 1.0.9 ARMv7 build CANNOT be installed here.\n");
            s.append("That is a firmware limitation, not an APK defect.\n");
        }

        s.append("\ncores: ").append(Runtime.getRuntime().availableProcessors()).append("\n");

        TextView tv = new TextView(this);
        tv.setText(s.toString());
        tv.setTextSize(11);
        tv.setPadding(24, 24, 24, 24);
        tv.setTextIsSelectable(true);
        ScrollView sv = new ScrollView(this);
        sv.addView(tv);
        setContentView(sv);
    }
}
