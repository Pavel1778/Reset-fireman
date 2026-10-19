package com.fireman.exectest;

import android.app.Activity;
import android.os.Build;
import android.os.Bundle;
import android.widget.ScrollView;
import android.widget.TextView;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.RandomAccessFile;
import java.io.StringWriter;
import java.lang.reflect.Method;
import java.util.Arrays;

/**
 * Answers the question that decides whether any 32-bit path can work here:
 * can the kernel actually execute AArch32 code?
 *
 * The firmware is 64-bit-only in userspace, so Package Manager refuses to
 * install an APK carrying armeabi-v7a libraries. That is a userspace policy. It
 * says nothing about whether the kernel can exec a 32-bit ELF, and a container
 * or VM app that brings its own 32-bit Android userspace depends entirely on
 * that kernel ability.
 *
 * This APK ships no native libraries, so it installs on any device. It carries
 * two tiny static ELFs as assets and runs both:
 *
 *   armprobe32.bin  ELF32/ARM      exit_group(42)
 *   armprobe64.bin  ELF64/AArch64  exit_group(43)   control
 *
 * Two things make the run possible where a naive attempt fails:
 *
 *  1. targetSdkVersion is 28. SELinux picks an app's domain from targetSdk
 *     (seapp_contexts). untrusted_app_29 and above carry no rule permitting exec
 *     of app_data_file, which is why running a binary from the app's own
 *     directory fails with EACCES even at mode 0755 while the kernel could exec
 *     it. untrusted_app_27, used for targetSdk <= 28, does have
 *     "allow untrusted_app_27 app_data_file:file execute_no_trans".
 *     Targeting 28 is exactly how Termux runs its own binaries.
 *
 *  2. The probes are static, single PT_LOAD, p_align 0x1000, so the loader has
 *     nothing else to object to.
 *
 * Results:
 *   32-bit 42 and 64-bit 43  -> kernel executes both; a 32-bit guest userspace
 *                               can run natively, with no CPU emulation.
 *   32-bit fails, 64-bit 43  -> kernel refuses AArch32; only a real rebuild to
 *                               arm64 can help.
 *   both fail                -> exec is blocked even at targetSdk 28; the error
 *                               line above says which failure it is.
 */
public class MainActivity extends Activity {

    private static final int STATUS_ARM32 = 42;
    private static final int STATUS_ARM64 = 43;

    /** Paths that exist only when the firmware ships a 32-bit runtime. */
    private static final String[] RUNTIME_32 = {
            "/system/bin/linker",
            "/system/lib/libc.so",
            "/system/lib/libart.so",
            "/apex/com.android.runtime/lib/bionic/libc.so",
            "/apex/com.android.runtime/lib/libc.so",
            "/apex/com.android.art/lib/libart.so",
    };

    /** The 64-bit counterparts, as a control for the checks above. */
    private static final String[] RUNTIME_64 = {
            "/system/bin/linker64",
            "/system/lib64/libc.so",
            "/apex/com.android.runtime/lib64/bionic/libc.so",
    };

    @Override
    protected void onCreate(Bundle b) {
        super.onCreate(b);
        StringBuilder sb = new StringBuilder();

        sb.append("ARM32 EXECUTION TEST\n====================\n\n");
        sb.append("Android ").append(Build.VERSION.RELEASE)
          .append(" (API ").append(Build.VERSION.SDK_INT).append(")\n");
        sb.append("Device: ").append(Build.MANUFACTURER).append(' ').append(Build.MODEL).append('\n');
        sb.append("Supported ABIs: ").append(Arrays.toString(Build.SUPPORTED_ABIS)).append('\n');
        sb.append("32-bit ABIs: ").append(Arrays.toString(Build.SUPPORTED_32_BIT_ABIS)).append('\n');
        sb.append("64-bit ABIs: ").append(Arrays.toString(Build.SUPPORTED_64_BIT_ABIS)).append('\n');
        sb.append("targetSdk of this app: ").append(getApplicationInfo().targetSdkVersion)
          .append("   (28 = allowed to exec its own files)\n");
        sb.append("SELinux domain: ").append(readAttr("current"))
          .append("   (expect untrusted_app_27 at targetSdk 28)\n");
        sb.append("zygote: ").append(prop("ro.zygote"))
          .append("   abilist: ").append(prop("ro.product.cpu.abilist")).append('\n');
        sb.append("dataDir: ").append(getApplicationInfo().dataDir).append('\n');
        sb.append("nativeLibraryDir: ").append(getApplicationInfo().nativeLibraryDir).append("\n\n");

        sb.append(scanRuntime("32-bit runtime files", RUNTIME_32));
        sb.append(scanRuntime("64-bit runtime files", RUNTIME_64));
        sb.append(readProcConfig());

        File dir = new File(getFilesDir(), "probe");
        if (!dir.exists() && !dir.mkdirs()) {
            sb.append("Could not create probe dir\n");
        }

        Result r32 = run("armprobe32.bin", dir);
        Result r64 = run("armprobe64.bin", dir);

        sb.append("\n32-bit (ELF32/ARM, exit ").append(STATUS_ARM32).append("):\n");
        sb.append(r32.describe(STATUS_ARM32)).append('\n');
        sb.append("64-bit control (ELF64/AArch64, exit ").append(STATUS_ARM64).append("):\n");
        sb.append(r64.describe(STATUS_ARM64)).append('\n');

        boolean ok32 = r32.exitCode == STATUS_ARM32;
        boolean ok64 = r64.exitCode == STATUS_ARM64;

        sb.append("VERDICT\n-------\n");
        if (ok32 && ok64) {
            sb.append("KERNEL RUNS 32-BIT CODE.\n\n");
            sb.append("The 64-bit-only restriction is userspace only: there is no\n");
            sb.append("32-bit zygote, so Package Manager offers no 32-bit ABI.\n");
            sb.append("The kernel underneath executes AArch32 natively.\n\n");
            sb.append("So a virtualisation app that supplies its own 32-bit\n");
            sb.append("Android (VMOS Pro with a 32-bit ROM, VPhoneOS, F1 VM)\n");
            sb.append("can run Fireman 1.0.9 on this phone, on the same cores,\n");
            sb.append("with no CPU emulation. Install the game inside the guest.");
        } else if (!ok32 && ok64) {
            sb.append("Kernel runs 64-bit but REFUSES 32-bit code.\n\n");
            sb.append("No VM or container can help. The only route is rebuilding\n");
            sb.append("the game with a 64-bit Unity runtime (IL2CPP + ARM64).");
        } else if (ok32 && !ok64) {
            sb.append("32-bit ran but the 64-bit control did not - unexpected.\n");
            sb.append("Please send this screen as-is.");
        } else {
            sb.append("Neither binary ran, so exec is blocked even at targetSdk 28.\n");
            sb.append("This OEM firmware differs from AOSP policy. Send this\n");
            sb.append("screen as-is; the error line above says which failure it is.");
        }

        TextView t = new TextView(this);
        t.setPadding(40, 40, 40, 40);
        t.setTextSize(13);
        t.setTextIsSelectable(true);
        t.setText(sb.toString());

        ScrollView sv = new ScrollView(this);
        sv.addView(t);
        setContentView(sv);
    }

    private static final class Result {
        int exitCode = -1;
        String error = null;
        String note = null;

        String describe(int expected) {
            StringBuilder s = new StringBuilder();
            if (error != null) {
                s.append("  could not run: ").append(error).append('\n');
            } else {
                s.append("  exit code: ").append(exitCode)
                 .append(exitCode == expected ? "   <- expected, execution worked"
                                              : "   <- unexpected")
                 .append('\n');
            }
            if (note != null && !note.isEmpty()) {
                s.append("  ").append(note).append('\n');
            }
            return s.toString();
        }
    }

    private Result run(String asset, File dir) {
        Result r = new Result();
        File out = new File(dir, asset);
        try {
            copyAsset(asset, out);
            boolean marked = out.setExecutable(true, false);
            r.note = "mode=" + (out.canExecute() ? "executable" : "NOT executable")
                   + (marked ? "" : ", setExecutable returned false");

            Process p = new ProcessBuilder(out.getAbsolutePath())
                    .redirectErrorStream(true).start();
            StringWriter sw = new StringWriter();
            try (BufferedReader br = new BufferedReader(new InputStreamReader(p.getInputStream()))) {
                String line;
                while ((line = br.readLine()) != null) {
                    sw.append(line).append('\n');
                }
            }
            r.exitCode = p.waitFor();
            String text = sw.toString().trim();
            if (!text.isEmpty()) {
                r.note += ", output=" + text;
            }
        } catch (Exception e) {
            r.error = e.getClass().getSimpleName() + ": " + e.getMessage();
        }
        return r;
    }

    private void copyAsset(String name, File out) throws Exception {
        try (InputStream in = getAssets().open(name);
             OutputStream os = new FileOutputStream(out)) {
            byte[] buf = new byte[4096];
            int n;
            while ((n = in.read(buf)) > 0) {
                os.write(buf, 0, n);
            }
        }
    }

    private String scanRuntime(String title, String[] paths) {
        StringBuilder sb = new StringBuilder(title).append(":\n");
        boolean any = false;
        for (String p : paths) {
            String info = elfInfo(p);
            if (info != null) {
                sb.append("  ").append(p).append("  ").append(info).append('\n');
                any = true;
            }
        }
        if (!any) {
            sb.append("  none present\n");
        }
        return sb.append('\n').toString();
    }

    /** Returns "ELF32/ARM" style text, or null when the path does not exist. */
    private static String elfInfo(String path) {
        File f = new File(path);
        if (!f.exists()) {
            return null;
        }
        try (RandomAccessFile raf = new RandomAccessFile(f, "r")) {
            byte[] h = new byte[20];
            if (raf.read(h) < 20) {
                return "too short to be ELF";
            }
            if (h[0] != 0x7f || h[1] != 'E' || h[2] != 'L' || h[3] != 'F') {
                return "not an ELF file";
            }
            int cls = h[4] & 0xff;
            int mach = (h[18] & 0xff) | ((h[19] & 0xff) << 8);
            String c = cls == 1 ? "ELF32" : cls == 2 ? "ELF64" : "class " + cls;
            String m = mach == 40 ? "ARM" : mach == 183 ? "AArch64" : "machine " + mach;
            return c + "/" + m;
        } catch (Exception e) {
            return "unreadable (" + e.getClass().getSimpleName() + ")";
        }
    }

    /**
     * /proc/config.gz is the ground truth for CONFIG_COMPAT when readable. It is
     * often blocked for apps, which is fine: the stock LJ7 kernel config is
     * already known and published alongside this APK.
     */
    private String readProcConfig() {
        StringBuilder sb = new StringBuilder("/proc/config.gz:\n");
        File f = new File("/proc/config.gz");
        if (!f.exists()) {
            return sb.append("  not present\n\n").toString();
        }
        if (!f.canRead()) {
            return sb.append("  present but not readable by apps (expected)\n\n").toString();
        }
        try (BufferedReader br = new BufferedReader(new InputStreamReader(
                new java.util.zip.GZIPInputStream(new java.io.FileInputStream(f))))) {
            String line;
            boolean found = false;
            while ((line = br.readLine()) != null) {
                if (line.startsWith("CONFIG_COMPAT")
                        || line.startsWith("CONFIG_ARM64_4K_PAGES")
                        || line.startsWith("CONFIG_KUSER_HELPERS")
                        || line.startsWith("CONFIG_ARM64_VA_BITS=")) {
                    sb.append("  ").append(line).append('\n');
                    found = true;
                }
            }
            if (!found) {
                sb.append("  readable but no CONFIG_COMPAT lines found\n");
            }
        } catch (Exception e) {
            sb.append("  read failed: ").append(e).append('\n');
        }
        return sb.append('\n').toString();
    }

    private static String prop(String key) {
        try {
            Class<?> c = Class.forName("android.os.SystemProperties");
            Method m = c.getMethod("get", String.class);
            Object v = m.invoke(null, key);
            return v == null ? "(empty)" : v.toString();
        } catch (Throwable t) {
            return "(unavailable)";
        }
    }

    /**
     * Reads a line from /proc/self/attr/<name>. "current" holds this process's
     * SELinux label, which is the direct evidence that targetSdk 28 put us in
     * untrusted_app_27 rather than a domain that cannot exec its own files.
     */
    private static String readAttr(String name) {
        try (BufferedReader br = new BufferedReader(
                new InputStreamReader(new java.io.FileInputStream("/proc/self/attr/" + name)))) {
            String line = br.readLine();
            return line == null ? "(empty)" : line.trim();
        } catch (Throwable t) {
            return "(unavailable: " + t.getClass().getSimpleName() + ")";
        }
    }
}
