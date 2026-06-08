# Python Governance Wrapper

[English](index.md)

`aikernel-governance` は、AIKernel.Control の public governance surface を
Python から利用するための distribution です。

これは C# package の wrapper であり、Control を Python で再実装するものでは
ありません。package は managed assemblies を同梱し、単一の import surface を
公開します。

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

```bash
pip install aikernel-governance
```

distribution name は `aikernel-governance` です。import name は
`aikernel_governance` です。

## Scope

package は public Control contract と public wrapper types を公開します。

- `ExecutionRequest`
- `ExecutionResult`
- `Snapshot`
- `SnapshotMetadata`
- `ProviderContract`
- `GovernanceClient`
- Bonsai provider、model、tokenizer、model-state wrapper
- Emulator graph、node、scheduler、policy、engine wrapper
- CPU kernel wrapper
- Diagnostics replay approval wrapper
- GPU delegate contract loader

internal governance engine helper、transport-specific code、OS-specific
implementation、private runtime internals は公開しません。

## Managed Assemblies

wheel は Control と contract assemblies を `aikernel_governance/native` に
同梱します。

- `AIKernel.Abstractions.dll`
- `AIKernel.Dtos.dll`
- `AIKernel.Enums.dll`
- `AIKernel.Control.Core.dll`
- `AIKernel.Control.CPU.dll`
- `AIKernel.Control.Diagnostics.dll`
- `AIKernel.Control.Emulator.dll`
- `AIKernel.Control.GPU.dll`

`governance_assemblies()` は、同梱 assembly、`AIKERNEL_GOVERNANCE_ASSEMBLY_PATH`、
NuGet global packages cache の順に assembly を解決します。

`load_governance_runtime()` は、解決した assembly を pythonnet 経由で読み込みます。

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

## API Example

```python
from aikernel_governance import ExecutionRequest, GovernanceClient

request = ExecutionRequest(
    model="bonsai-1.7b",
    input="hello",
    parameters={"execution_id": "exec-001"},
)

client = GovernanceClient(backend)
result = client.submit(request)
```

`GovernanceClient` は public backend に委譲します。backend は
`submit(request)`、`snapshot(id)`、`result(id)` を公開する必要があります。
