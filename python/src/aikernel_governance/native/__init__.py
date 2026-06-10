"""[EN]
Managed assembly discovery and pythonnet loading for AIKernel.Control.

[JA]
AIKernel.Control の managed assembly 探索と pythonnet 読み込みを提供します。
"""

from __future__ import annotations

import os
from dataclasses import dataclass
from pathlib import Path


_CONTROL_PACKAGE_VERSION = "0.1.1"
_CONTRACT_PACKAGE_VERSION = "0.1.1"
_ASSEMBLIES = (
    "AIKernel.Abstractions.dll",
    "AIKernel.Dtos.dll",
    "AIKernel.Enums.dll",
    "AIKernel.Control.Core.dll",
    "AIKernel.Control.CPU.dll",
    "AIKernel.Control.Diagnostics.dll",
    "AIKernel.Control.Emulator.dll",
    "AIKernel.Control.GPU.dll",
)
_ASSEMBLY_PACKAGES = {
    "AIKernel.Abstractions.dll": ("AIKernel.Abstractions", _CONTRACT_PACKAGE_VERSION),
    "AIKernel.Dtos.dll": ("AIKernel.Dtos", _CONTRACT_PACKAGE_VERSION),
    "AIKernel.Enums.dll": ("AIKernel.Enums", _CONTRACT_PACKAGE_VERSION),
    "AIKernel.Control.Core.dll": ("AIKernel.Control.Core", _CONTROL_PACKAGE_VERSION),
    "AIKernel.Control.CPU.dll": ("AIKernel.Control.CPU", _CONTROL_PACKAGE_VERSION),
    "AIKernel.Control.Diagnostics.dll": ("AIKernel.Control.Diagnostics", _CONTROL_PACKAGE_VERSION),
    "AIKernel.Control.Emulator.dll": ("AIKernel.Control.Emulator", _CONTROL_PACKAGE_VERSION),
    "AIKernel.Control.GPU.dll": ("AIKernel.Control.GPU", _CONTROL_PACKAGE_VERSION),
}


@dataclass(frozen=True)
class GovernanceAssemblySet:
    """[EN]
    Resolved managed assemblies that define the governance contract boundary.

    [JA]
    governance 契約境界を定義する解決済み managed assembly 群を表します。
    """

    root: Path
    assemblies: tuple[Path, ...]

    @property
    def is_complete(self) -> bool:
        """[EN]
        Return whether every expected assembly path exists.

        [JA]
        期待されるすべての assembly パスが存在するかを返します。
        """
        return all(path.exists() for path in self.assemblies)

    @property
    def missing(self) -> tuple[str, ...]:
        """[EN]
        Return missing assembly file names.

        [JA]
        不足している assembly ファイル名を返します。
        """
        return tuple(path.name for path in self.assemblies if not path.exists())


def governance_assemblies() -> GovernanceAssemblySet:
    """[EN]
    Resolve the bundled or NuGet-provided governance assemblies.

    [JA]
    同梱または NuGet 提供の governance assembly を解決します。
    """
    root = _native_root()
    return GovernanceAssemblySet(
        root=root,
        assemblies=tuple(_resolve_assembly(name) for name in _ASSEMBLIES),
    )


def require_governance_assemblies() -> GovernanceAssemblySet:
    """[EN]
    Resolve assemblies and fail if any required assembly is missing.

    [JA]
    assembly を解決し、必要な assembly が不足している場合は失敗します。
    """
    assemblies = governance_assemblies()
    if not assemblies.is_complete:
        missing = ", ".join(assemblies.missing)
        raise FileNotFoundError(
            "AIKernel.Control managed assemblies are not available: "
            f"{missing}. Bundle them in aikernel_governance/native or restore "
            "the corresponding NuGet packages."
        )

    return assemblies


def load_governance_runtime() -> GovernanceAssemblySet:
    """[EN]
    Load bundled C# assemblies through pythonnet and return their paths.

    [JA]
    同梱 C# assembly を pythonnet 経由で読み込み、そのパスを返します。
    """

    assemblies = require_governance_assemblies()
    try:
        from pythonnet import load  # type: ignore[import-not-found]

        try:
            load("coreclr")
        except RuntimeError:
            pass
        import clr  # type: ignore[import-not-found]
    except ImportError as exc:
        raise RuntimeError(
            "pythonnet is required to load AIKernel.Control assemblies."
        ) from exc

    for assembly in assemblies.assemblies:
        clr.AddReference(str(assembly))

    return assemblies


def _resolve_assembly(name: str) -> Path:
    for root in _assembly_roots():
        candidate = root / name
        if candidate.exists():
            return candidate

    nuget_candidate = _resolve_nuget_assembly(name)
    if nuget_candidate is not None:
        return nuget_candidate

    return _native_root() / name


def _assembly_roots() -> tuple[Path, ...]:
    roots = [_native_root()]
    override = os.environ.get("AIKERNEL_GOVERNANCE_ASSEMBLY_PATH")
    if override:
        roots.extend(Path(path) for path in override.split(os.pathsep) if path)
    return tuple(roots)


def _native_root() -> Path:
    return Path(__file__).resolve().parent


def _resolve_nuget_assembly(name: str) -> Path | None:
    package, version = _ASSEMBLY_PACKAGES[name]
    candidate = _nuget_root() / package.lower() / version / "lib" / "net10.0" / name
    if candidate.exists():
        return candidate
    return None


def _nuget_root() -> Path:
    configured = os.environ.get("NUGET_PACKAGES")
    if configured:
        return Path(configured)
    return Path.home() / ".nuget" / "packages"
