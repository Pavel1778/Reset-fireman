#!/usr/bin/env bash
# Install the APK analysis/repack tooling used in this repo.
#
# build-tools (aapt2, apksigner, zipalign, d8, dexdump) already live in sdk/ and
# a JDK is expected at $JAVA_HOME. This adds what is missing: apktool for
# decode/rebuild, jadx for Java decompilation, and the Python libraries used for
# ELF and dex inspection.
#
# Tools go to ~/apktools/bin because /opt is not writable here. The wrapper is
# added to ~/.bashrc so later shells pick it up.
set -eu

BIN="$HOME/apktools/bin"
mkdir -p "$BIN"

if [ ! -f "$HOME/apktools/apktool.jar" ]; then
  echo "fetching apktool"
  curl -sL -o "$HOME/apktools/apktool.jar" \
    "https://github.com/iBotPeaches/Apktool/releases/download/v2.11.1/apktool_2.11.1.jar"
fi

if [ ! -d "$HOME/apktools/jadx" ]; then
  echo "fetching jadx"
  curl -sL -o "$HOME/apktools/jadx.zip" \
    "https://github.com/skylot/jadx/releases/download/v1.5.1/jadx-1.5.1.zip"
  python3 -c "import zipfile; zipfile.ZipFile('$HOME/apktools/jadx.zip').extractall('$HOME/apktools/jadx')"
fi

printf '#!/usr/bin/env bash\nexec java -jar "$HOME/apktools/apktool.jar" "$@"\n' > "$BIN/apktool"
printf '#!/usr/bin/env bash\nexec "$HOME/apktools/jadx/bin/jadx" "$@"\n' > "$BIN/jadx"
chmod +x "$BIN/apktool" "$BIN/jadx"

python3 -m pip install --quiet lief androguard capstone

grep -q 'apktools/bin' "$HOME/.bashrc" 2>/dev/null \
  || printf '\n# APK tooling\nexport PATH="$HOME/apktools/bin:$PATH"\n' >> "$HOME/.bashrc"

echo "installed:"
"$BIN/apktool" --version
"$BIN/jadx" --version
python3 -c "import lief, androguard, capstone; print('lief', lief.__version__, '| androguard', androguard.__version__, '| capstone', capstone.__version__)"
