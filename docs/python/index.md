# Python Governance Wrapper

[日本語](index-ja.md)

`aikernel-governance` was designed as the Python distribution for the public
AIKernel.Control governance surface.

For the 0.1.1.1 update line, AIKernel.Control is NuGet-only. Do not build,
install, or publish a PyPI package for this line. This page is retained as
reference documentation for a future explicitly scheduled Python release.

The future wrapper design sits over the C# packages; it is not a Python
reimplementation of Control. It should expose a single import surface:

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

There is no supported install command for 0.1.1.1. The archived distribution
name is `aikernel-governance`; the archived import name is
`aikernel_governance`.

## Scope

The archived package design exposes public Control contracts and public wrapper
types:

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

The archived wheel design bundles the Control and contract assemblies under
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

Do not run Python build or publish commands for 0.1.1.1. Validate the NuGet
surface instead:

```powershell
dotnet build AIKernel.Control.slnx -c Release -p:WarningsAsErrors=1591
dotnet test AIKernel.Control.slnx -c Release --no-build
dotnet pack AIKernel.Control.slnx -c Release --no-build --no-restore -p:UseLocalPackageVersion=true -p:LocalPackageBuildNumber=3 -o ..\artifacts\local-packages
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

Python must remain a thin managed-call wrapper. It must not implement CTG Gate
rules, approve-count logic, veto behavior, or trajectory halt aggregation.
