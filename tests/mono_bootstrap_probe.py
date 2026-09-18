"""Load the production Loader + real embedded Harmony in an isolated game Mono.

The engine is a fixture: no game process, registry, vault or actual Unity method is accessed.
"""
import ctypes as c
import os
import sys
from pathlib import Path

base = Path(sys.argv[1]).resolve()
out = Path(sys.argv[2]).resolve()
out.mkdir(parents=True, exist_ok=True)
os.environ["GACHA_PROBE_OUTPUT"] = str(out)
embed = base / "MonoBleedingEdge/EmbedRuntime"
handle = os.add_dll_directory(str(embed))
mono = c.CDLL(str(embed / "mono-2.0-bdwgc.dll"))


def call(name, result, arguments):
    function = getattr(mono, name)
    function.restype, function.argtypes = result, arguments
    return function


managed = str(base / "BrownDust II_Data/Managed").encode()
call("mono_set_dirs", None, [c.c_char_p, c.c_char_p])(
    managed, str(base / "MonoBleedingEdge/etc").encode()
)
call("mono_set_assemblies_path", None, [c.c_char_p])(managed)
call("mono_config_parse", None, [c.c_char_p])(None)
domain = call("mono_jit_init_version", c.c_void_p, [c.c_char_p, c.c_char_p])(
    b"gacha-bootstrap-probe", b"v4.0.30319"
)
assert domain
runner = str(out / "Probe.exe").encode()
assembly = call("mono_domain_assembly_open", c.c_void_p, [c.c_void_p, c.c_char_p])(
    domain, runner
)
assert assembly
arguments = (c.c_char_p * 2)(runner, str(out / "bootstrap.dll").encode())
result = call("mono_jit_exec", c.c_int, [c.c_void_p, c.c_void_p, c.c_int, c.POINTER(c.c_char_p)])(
    domain, assembly, 2, arguments
)
status = (out / "runtime.json").read_text(encoding="utf-8")
passed = result == 0 and status == "active|"
print("PASS: isolated Mono Loader + real Harmony" if passed else f"FAIL: {result}, {status}", flush=True)
os._exit(0 if passed else 1)
