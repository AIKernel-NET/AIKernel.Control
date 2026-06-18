# aikernel-governance

[English](README.md)

AIKernel.Control の public governance surface を Python から扱うための wrapper
surface です。

0.1.2 正典系列から、`aikernel-governance` は AIKernel.Control の public
governance boundary を公開する PyPI package です。この package は managed C#
assembly の薄い wrapper であり、CTG Gate logic を Python 側で再実装しません。

## Scope

`aikernel-governance` は、AIKernel.Control の public contract boundary を単一の
Python API として公開します。

- execution request、result、snapshot envelope
- provider contract metadata
- Bonsai provider、tokenizer、model config、model-state wrapper
- emulator graph、node、scheduler、policy、engine wrapper
- CPU kernel wrapper
- diagnostics replay approval wrapper
- GPU delegate contract loader
- managed assembly discovery と pythonnet loading

この参照 package は governance semantics を Python で別実装しません。
internal engine helper、transport-specific logic、OS-specific implementation、
private runtime state は公開しません。

## Managed Assemblies

Python package は、public Control と contract assemblies を
`aikernel_governance/native` で解決します。

- `AIKernel.Abstractions.dll`
- `AIKernel.Dtos.dll`
- `AIKernel.Enums.dll`
- `AIKernel.Control.Core.dll`
- `AIKernel.Control.CPU.dll`
- `AIKernel.Control.Diagnostics.dll`
- `AIKernel.Control.Emulator.dll`
- `AIKernel.Control.GPU.dll`

`governance_assemblies()` は、同梱 assembly、`AIKERNEL_GOVERNANCE_ASSEMBLY_PATH`、
NuGet global-packages cache の順に assembly を解決する想定です。

`load_governance_runtime()` は、解決した assembly を pythonnet 経由で読み込みます。

## Managed API Catalog

v0.1.2 package では generated managed API catalog を公開します。
`managed_api_catalog()`、`managed_api_summary()`、`managed_type_names()`、
`find_managed_type(full_name)` で確認できます。

## API

```python
from aikernel_governance import (
    ExecutionRequest,
    ExecutionResult,
    Snapshot,
    ProviderContract,
    GovernanceClient,
)
```

Example:

```python
from aikernel_governance import ExecutionRequest, GovernanceClient

request = ExecutionRequest(
    model="bonsai-1.7b",
    input="hello",
    parameters={"execution_id": "exec-001"},
)

client = GovernanceClient(backend)
result = client.submit(request)
snapshot = client.snapshot("exec-001")
```

`backend` は client が使用する governance operation を公開する必要があります。

- `submit(request)`
- `snapshot(id)`
- `result(id)`

pythonnet が利用できる場合、wrapper は `to_managed()` によって public C# DTO へ
変換できます。

## Build and Validate

local validation では Python wrapper の contract test を実行します。

```powershell
cd C:\Users\HP\source\repos\AIKernel-NET\AIKernel.Control
py -m pytest python\tests
```

## Distribution

PyPI publishing は repository の GitHub Actions workflow が release tag を契機に
Trusted Publishing で実行します。Python 側の試作を行う場合も managed assemblies
の薄い wrapper に留め、CTG Gate rule を Python code に追加しないでください。
