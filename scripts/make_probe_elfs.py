#!/usr/bin/env python3
"""Generate minimal static ARM ELF executables used to probe kernel AArch32 support.

The LJ7 firmware is 64-bit-only in userspace, but that says nothing about whether
the *kernel* can execute 32-bit ELF binaries (CONFIG_COMPAT). A 32-bit Android
userspace can only ever be provided by a container/VM app, and such an app still
needs the host kernel to execute AArch32 code.

So the decisive question is a kernel question, and it can be answered by asking
the kernel to run a 32-bit binary.

These binaries are deliberately trivial: a single exit() syscall with a distinct
status, no libc, no dynamic linker, no relocations. If the kernel refuses to
exec them it is not because of anything in the binary.

  arm32  -> exit(42)
  arm64  -> exit(43)
"""
import struct
import sys

EM_ARM = 40
EM_AARCH64 = 183
ET_DYN = 3  # static PIE; Android wants position-independent executables
PT_LOAD = 1
PF_R = 4
PF_X = 1


def _code_arm32(status):
    # mov r7, #248   (exit_group)  -> 0xE3A070F8
    # mov r0, #status              -> 0xE3A00000 | status
    # svc #0                       -> 0xEF000000
    return struct.pack("<III", 0xE3A070F8, 0xE3A00000 | status, 0xEF000000)


def _code_aarch64(status):
    # mov x0, #status   movz x0,#imm -> 0xD2800000 | (imm<<5)
    # mov x8, #94       (exit_group) -> 0xD2800000 | (94<<5) | 8
    # svc #0                         -> 0xD4000001
    return struct.pack(
        "<III",
        0xD2800000 | (status << 5),
        0xD2800000 | (94 << 5) | 8,
        0xD4000001,
    )


def build_arm32(status=42):
    code = _code_arm32(status)
    ehsize, phentsize = 52, 32
    phoff = ehsize
    entry = phoff + phentsize
    total = entry + len(code)
    e_ident = b"\x7fELF" + bytes([1, 1, 1, 0]) + b"\x00" * 8  # 32-bit, LE
    ehdr = e_ident + struct.pack(
        "<HHIIIIIHHHHHH",
        ET_DYN, EM_ARM, 1, entry, phoff, 0, 0x05000000,  # EF_ARM_EABI_VER5
        ehsize, phentsize, 1, 0, 0, 0,
    )
    phdr = struct.pack(
        "<IIIIIIII",
        PT_LOAD, 0, 0, 0, total, total, PF_R | PF_X, 0x1000,
    )
    return ehdr + phdr + code


def build_aarch64(status=43):
    code = _code_aarch64(status)
    ehsize, phentsize = 64, 56
    phoff = ehsize
    entry = phoff + phentsize
    total = entry + len(code)
    e_ident = b"\x7fELF" + bytes([2, 1, 1, 0]) + b"\x00" * 8  # 64-bit, LE
    ehdr = e_ident + struct.pack(
        "<HHIQQQIHHHHHH",
        ET_DYN, EM_AARCH64, 1, entry, phoff, 0, 0,
        ehsize, phentsize, 1, 0, 0, 0,
    )
    phdr = struct.pack(
        "<IIQQQQQQ",
        PT_LOAD, PF_R | PF_X, 0, 0, 0, total, total, 0x1000,
    )
    return ehdr + phdr + code


def describe(data, label):
    is64 = data[4] == 2
    machine = struct.unpack_from("<H", data, 18)[0]
    etype = struct.unpack_from("<H", data, 16)[0]
    entry = struct.unpack_from("<Q", data, 24)[0] if is64 else struct.unpack_from("<I", data, 24)[0]
    phoff = struct.unpack_from("<Q", data, 32)[0] if is64 else struct.unpack_from("<I", data, 28)[0]
    phnum = struct.unpack_from("<H", data, 56 if is64 else 44)[0]
    print(f"  {label}: class={'ELF64' if is64 else 'ELF32'} type={etype} "
          f"machine={machine} entry=0x{entry:x} phoff={phoff} phnum={phnum} size={len(data)}")
    return is64, machine


def main():
    out = sys.argv[1] if len(sys.argv) > 1 else "."
    import os
    a32 = build_arm32(42)
    a64 = build_aarch64(43)
    os.makedirs(out, exist_ok=True)
    p32 = os.path.join(out, "libarmprobe32.so")
    p64 = os.path.join(out, "libarmprobe64.so")
    with open(p32, "wb") as f:
        f.write(a32)
    with open(p64, "wb") as f:
        f.write(a64)
    print("generated:")
    describe(a32, "arm32")
    describe(a64, "arm64")
    print("  paths:", p32, p64)


if __name__ == "__main__":
    main()
