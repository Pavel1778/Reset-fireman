#!/usr/bin/env python3
"""Build control APKs that isolate Package Manager checks from native-code loading.

Control A: the real game (all assets, real manifest) with native libs removed.
  - Installs      => Package Manager accepted everything; failure is ABI-only.
  - Fails to install => something else in manifest/resources is rejected first.

Control B: same as A but also relaxes the one REQUIRED uses-feature.
"""
import os
import subprocess
import sys
import zipfile

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
BT = os.path.join(ROOT, "sdk/build-tools/34.0.0")
JAVA_HOME = os.environ.get("JAVA_HOME", "/tmp/jdk/jdk-17.0.20.1+1")
SRC = os.path.join(ROOT, "work/orig_fireman.apk")
KS = os.path.join(ROOT, "work/fireman-release.keystore")
KS_PASS = os.environ.get("FIREMAN_KS_PASS", "fireman123")
ENV = {**os.environ, "JAVA_HOME": JAVA_HOME, "PATH": JAVA_HOME + "/bin:" + os.environ["PATH"]}


def repack(src, dst, drop_libs=True):
    zin = zipfile.ZipFile(src)
    zout = zipfile.ZipFile(dst, "w", zipfile.ZIP_DEFLATED)
    kept = dropped = 0
    for i in zin.infolist():
        if i.filename.startswith("lib/"):
            dropped += 1
            continue
        if i.filename.upper().startswith("META-INF/"):
            continue  # old signature is invalid once we repack
        zi = zipfile.ZipInfo(i.filename, date_time=i.date_time)
        zi.compress_type = i.compress_type
        zi.external_attr = i.external_attr
        zout.writestr(zi, zin.read(i.filename))
        kept += 1
    zout.close()
    return kept, dropped


def sign(unsigned, out):
    aligned = unsigned + ".aligned"
    subprocess.run([BT + "/zipalign", "-p", "-f", "4", unsigned, aligned], check=True)
    subprocess.run([
        BT + "/apksigner", "sign",
        "--ks", KS, "--ks-pass", "pass:" + KS_PASS, "--key-pass", "pass:" + KS_PASS,
        "--v1-signing-enabled", "true", "--v2-signing-enabled", "true",
        "--v3-signing-enabled", "true", "--out", out, aligned,
    ], check=True, env=ENV)


def main():
    outdir = sys.argv[1] if len(sys.argv) > 1 else "/tmp"
    unsigned = os.path.join(outdir, "controlA_unsigned.apk")
    kept, dropped = repack(SRC, unsigned)
    print(f"repacked: {kept} entries kept, {dropped} libs dropped")
    final = os.path.join(outdir, "Fireman_Control_NoLibs.apk")
    sign(unsigned, final)
    print("built:", final, os.path.getsize(final), "bytes")


if __name__ == "__main__":
    main()
