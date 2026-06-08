# aikernel-governance

[English](README.md)

AIKernel.Control の public governance surface を Python から扱うための wrapper
package です。

stable package は PyPI に公開されます。

```bash
pip install aikernel-governance
```

distribution name は `aikernel-governance` です。module は
`aikernel_governance` として import します。

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

この package は governance semantics を Python で別実装しません。
internal engine helper、transport-specific logic、OS-specific implementation、
private runtime state は公開しません。

## Managed Assemblies

wheel は public Control と contract assemblies を `aikernel_governance/native`
に同梱します。

- `AIKernel.Abstractions.dll`
- `AIKernel.Dtos.dll`
- `AIKernel.Enums.dll`
- `AIKernel.Control.Core.dll`
- `AIKernel.Control.CPU.dll`
- `AIKernel.Control.Diagnostics.dll`
- `AIKernel.Control.Emulator.dll`
- `AIKernel.Control.GPU.dll`

`governance_assemblies()` は、同梱 assembly、`AIKERNEL_GOVERNANCE_ASSEMBLY_PATH`、
NuGet global-packages cache の順に assembly を解決します。

`load_governance_runtime()` は、解決した assembly を pythonnet 経由で読み込みます。

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

## Build

```powershell
cd C:\Users\HP\source\repos\AIKernel-NET\AIKernel.Control
dotnet test AIKernel.Control.slnx -c Release --no-restore
dotnet pack AIKernel.Control.slnx -c Release --no-restore
cd python
py -m pytest
py -m build --wheel
py -m twine check dist\aikernel_governance-0.1.0-py3-none-any.whl
```

## Source Validation

source-based local validation では、clean virtual environment を使用してください。

```bash
pip install --force-reinstall \
  git+https://github.com/AIKernel-NET/AIKernel.Control.git#subdirectory=python
```
