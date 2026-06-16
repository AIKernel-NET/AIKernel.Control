# Python Governance Wrapper

[English](index.md)

`aikernel-governance` は、v0.1.2 正典シリーズにおける AIKernel.Control の public
governance surface 用 Python distribution です。

この wrapper は C# packages の上にあります。Control や CTG を Python で再実装する
ものではありません。単一の import surface を公開します。

```python
from aikernel_governance import (
    ExecutionRequest,
    ExecutionResult,
    Snapshot,
    ProviderContract,
    GovernanceClient,
)
```

## Install

stable publication 後の install:

```bash
pip install aikernel-governance==0.1.2
```

local validation では `0.1.2.dev{buildNumber}` wheel を使います。release task が
明示的に要求するまで、stable `0.1.2` artifact は作成しません。

## Scope

package は public Control contract と public wrapper type を公開します。

- `ExecutionRequest`
- `ExecutionResult`
- `Snapshot`
- `SnapshotMetadata`
- `ProviderContract`
- `GovernanceClient`
- Bonsai provider、model、tokenizer、model-state wrappers
- Emulator graph、node、scheduler、policy、engine wrappers
- CPU kernel wrapper
- Diagnostics replay approval wrapper
- GPU delegate contract loader
- generated managed API catalog helpers

internal governance engine helper、transport-specific code、OS-specific
implementation、private runtime internals、CTG Gate rule は公開しません。

## Managed Assemblies

wheel は Control / contract assemblies を `aikernel_governance/native` 配下に同梱、
またはそこから解決します。

- `AIKernel.Abstractions.dll`
- `AIKernel.Dtos.dll`
- `AIKernel.Enums.dll`
- `AIKernel.Control.Core.dll`
- `AIKernel.Control.CPU.dll`
- `AIKernel.Control.Diagnostics.dll`
- `AIKernel.Control.Emulator.dll`
- `AIKernel.Control.GPU.dll`

`governance_assemblies()` は同梱 assembly、`AIKERNEL_GOVERNANCE_ASSEMBLY_PATH`、
NuGet global-packages cache の順に assembly を解決します。

`load_governance_runtime()` は解決した assemblies を pythonnet 経由で読み込みます。

## Managed API Catalog

v0.1.2 package では generated managed API catalog を公開します。
`managed_api_catalog()`、`managed_api_summary()`、`managed_type_names()`、
`find_managed_type(full_name)` で確認できます。

## Build

```powershell
py -m compileall python\src python\tests
py -m pytest python\tests
py -m build --wheel
```

PyPI publication は GitHub Actions Trusted Publishing と `pypi` environment で行います。
