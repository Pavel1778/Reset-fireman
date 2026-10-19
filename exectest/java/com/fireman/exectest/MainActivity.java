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
import java.io.StringWriter;
import java.util.Arrays;

/**
 * Answers one question that decides whether any 32-bit path can work on this phone:
 * can the kernel actually execute AArch32 code?
 *
 * The firmware is 64-bit-only in userspace, so Package Manager refuses to install an
 * APK carrying armeabi-v7a libraries. That is a userspace policy. It does not tell us
 * whether the kernel can exec a 32-bit ELF, and a container or VM app that supplies
 * its own 32-bit Android userspace depends entirely on that kernel ability.
 *
 * This APK has no native libraries, so it installs on any device. It ships two tiny
 * static ELFs as assets and runs both:
 *
 *   libarmprobe32.so  (ELF32/ARM, exit_group(42))
 *   libarmprobe64.so  (ELF64/AArch64, exit_group(43))
 *
 * The 64-bit one is a control: if it runs, exec of dropped files works at all. The
 * 32-bit one then isolates the AArch32 question.
 *
 * Results:
 *   32-bit exit 42 and 64-bit exit 43  -> kernel executes both; a 32-bit userspace
 *                                        (VM/container) can run natively.
 *   32-bit fails, 64-bit exit 43      -> kernel refuses AArch32; only a real
 *                                        recompilation to arm64 can help.
 *   both fail                          -> exec of dropped files is blocked by
 *                                        SELinux on this build.
 */
public class MainActivity extends Activity {

    private static final int STATUS_ARM32 = 42;
    private static final int STATUS_ARM64 = 43;

    @Override
    protected void onCreate(Bundle b) {
        super.onCreate(b);
        StringBuilder sb = new StringBuilder();

        sb.append("ARM32 EXECUTION TEST\n");
        sb.append("====================\n\n");
        sb.append("Android ").append(Build.VERSION.RELEASE)
          .append(" (API ").append(Build.VERSION.SDK_INT).append(")\n");
        sb.append("Device: ").append(Build.MANUFACTURER).append(" ").append(Build.MODEL).append("\n");
        sb.append("Supported ABIs: ").append(Arrays.toString(Build.SUPPORTED_ABIS)).append("\n");
        sb.append("32-bit ABIs: ").append(Arrays.toString(Build.SUPPORTED_32_BIT_ABIS)).append("\n");
        sb.append("64-bit ABIs: ").append(Arrays.toString(Build.SUPPORTED_64_BIT_ABIS)).append("\n\n");

        sb.append(readProcConfigCompat());

        File dir = new File(getFilesDir(), "probe");
        if (!dir.exists() && !dir.mkdirs()) {
            sb.append("Could not create probe dir\n");
        }

        Result r32 = run("armprobe32.bin", STATUS_ARM32, dir);
        Result r64 = run("armprobe64.bin", STATUS_ARM64, dir);

        sb.append("\n32-bit (ELF32/ARM, exit ").append(STATUS_ARM32).append("):\n");
        sb.append(r32.describe(STATUS_ARM32)).append("\n");
        sb.append("64-bit control (ELF64/AArch64, exit ").append(STATUS_ARM64).append("):\n");
        sb.append(r64.describe(STATUS_ARM64)).append("\n");

        boolean ok32 = r32.exitCode == STATUS_ARM32;
        boolean ok64 = r64.exitCode == STATUS_ARM64;

        sb.append("VERDICT\n-------\n");
        if (ok32 && ok64) {
            sb.append("KERNEL RUNS 32-BIT CODE (CONFIG_COMPAT works).\n");
            sb.append("A 32-bit userspace supplied by a VM/container app\n");
            sb.append("(VMOS Pro / VPhoneOS / F1 VM / X8 Sandbox) can run\n");
            sb.append("the game natively on this phone.");
        } else if (!ok32 && ok64) {
            sb.append("Kernel runs 64-bit but REFUSES 32-bit code.\n");
            sb.append("No VM/container can help; only rebuilding the game\n");
            sb.append("with a 64-bit Unity runtime can.");
        } else if (ok32 && !ok64) {
            sb.append("32-bit ran but the 64-bit control did not - unexpected.\n");
            sb.append("Re-run and send this screen as-is.");
        } else {
            sb.append("Neither binary ran, so exec of files in the app\n");
            sb.append("directory is blocked (SELinux). The AArch32 question\n");
            sb.append("is still open; try an app that is allowed to exec.");
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
        String note = "";

        String describe(int expected) {
            StringBuilder s = new StringBuilder();
            if (error != null) {
                s.append("  could not run: ").append(error).append("\n");
            } else {
                s.append("  exit code: ").append(exitCode);
                s.append(exitCode == expected ? "  <- expected, execution worked" : "  <- unexpected");
                s.append("\n");
            }
            if (note != null && !note.isEmpty()) {
                s.append("  note: ").append(note).append("\n");
            }
            return s.toString();
        }
    }

    private Result run(String asset, int expected, File dir) {
        Result r = new Result();
        File out = new File(dir, asset);
        try {
            copyAsset(asset, out);
            if (!out.setExecutable(true, false)) {
                r.note = "setExecutable(false) returned false";
            }
            try {
                Runtime.getRuntime().exec(new String[]{"chmod", "755", out.getAbsolutePath()}).waitFor();
            } catch (Exception ignored) {
                // chmod may be absent; setExecutable above is the primary path.
            }
            r.note = (r.note == null ? "" : r.note + "; ")
                    + "mode=" + (out.canExecute() ? "executable" : "NOT executable");

            Process p = new ProcessBuilder(out.getAbsolutePath()).redirectErrorStream(true).start();
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
                r.note += "; output=" + text;
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

    /**
     * /proc/config.gz exists when the kernel is built with CONFIG_IKCONFIG_PROC.
     * When readable, it is the ground truth for whether AArch32 support is compiled in.
     */
    private String readProcConfigCompat() {
        StringBuilder sb = new StringBuilder();
        File f = new File("/proc/config.gz");
        if (!f.exists()) {
            sb.append("/proc/config.gz: not present\n\n");
            return sb.toString();
        }
        if (!f.canRead()) {
            sb.append("/proc/config.gz: present but not readable by apps\n\n");
            return sb.toString();
        }
        try (BufferedReader br = new BufferedReader(new InputStreamReader(
                new java.util.zip.GZIPInputStream(new java.io.FileInputStream(f))))) {
            String line;
            while ((line = br.readLine()) != null) {
                if (line.startsWith("CONFIG_COMPAT")
                        || line.startsWith("CONFIG_ARM64_4K_PAGES")
                        || line.startsWith("CONFIG_KUSER_HELPERS")
                        || line.startsWith("CONFIG_ARM64_VA_BITS=")) {
                    sb.append(line).append('\n');
                }
            }
            sb.append('\n');
        } catch (Exception e) {
            sb.append("/proc/config.gz: read failed: ").append(e).append("\n\n");
        }
        return sb.toString();
    }
}
