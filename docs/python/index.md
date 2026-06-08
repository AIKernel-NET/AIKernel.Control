# Python Governance Wrapper

[日本語](index-ja.md)

`aikernel-governance` is the Python distribution for the public
AIKernel.Control governance surface.

It is a wrapper over the C# packages, not a Python reimplementation of Control.
The package bundles managed assemblies and exposes a single import surface:

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

The distribution name is `aikernel-governance`. The import name is
`aikernel_governance`.

## Scope

The package exposes public Control contracts and public wrapper types:

- `ExecutionRequest`
- `ExecutionResult`
- `Snapshot`
- `SnapshotMetadata`
- `ProviderContract`
- `GovernanceClient`
- Bonsai provider, model, tokenizer, and model-state wrappers
- Emulator graph, node, scheduler, policy, and engine wrappers
- CPU kernel wrapper
- Diagnostics replay approval wrapper
- GPU delegate contract loader

It does not expose internal governance engine helpers, transport-specific code,
OS-specific implementations, or private runtime internals.

## Managed Assemblies

The wheel bundles the Control and contract assemblies under
`aikernel_governance/native`:

- `AIKernel.Abstractions.dll`
- `AIKernel.Dtos.dll`
- `AIKernel.Enums.dll`
- `AIKernel.Control.Core.dll`
- `AIKernel.Control.CPU.dll`
- `AIKernel.Control.Diagnostics.dll`
- `AIKernel.Control.Emulator.dll`
- `AIKernel.Control.GPU.dll`

`governance_assemblies()` resolves bundled assemblies first, then paths from
`AIKERNEL_GOVERNANCE_ASSEMBLY_PATH`, then matching NuGet packages from the
global packages cache.

`load_governance_runtime()` loads the resolved assemblies through pythonnet.

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

`GovernanceClient` delegates to a public backend. The backend must expose
`submit(request)`, `snapshot(id)`, and `result(id)`.
