# aikernel-governance

[日本語](README-ja.md)

Reference design for the Python wrapper surface of AIKernel.Control.

For the 0.1.1.1 line, AIKernel.Control publishes NuGet packages only. This
directory is retained as a reference for the future Python wrapper surface and
is not built, installed, or published as a PyPI package.

The reserved distribution name is `aikernel-governance`. The intended module
name is `aikernel_governance`.

## Scope

`aikernel-governance` exposes the public AIKernel.Control contract boundary as a
single Python API:

- execution request, result, and snapshot envelopes
- provider contract metadata
- Bonsai provider, tokenizer, model config, and model-state wrappers
- emulator graph, node, scheduler, policy, and engine wrappers
- CPU kernel wrapper
- diagnostics replay approval wrapper
- GPU delegate contract loader
- managed assembly discovery and pythonnet loading

The reference package does not provide a separate Python implementation of
governance semantics. It does not expose internal engine helpers,
transport-specific logic, OS-specific implementations, or private runtime
state.

## Managed Assemblies

Future Python packaging would resolve the public Control and contract
assemblies under `aikernel_governance/native`:

- `AIKernel.Abstractions.dll`
- `AIKernel.Dtos.dll`
- `AIKernel.Enums.dll`
- `AIKernel.Control.Core.dll`
- `AIKernel.Control.CPU.dll`
- `AIKernel.Control.Diagnostics.dll`
- `AIKernel.Control.Emulator.dll`
- `AIKernel.Control.GPU.dll`

`governance_assemblies()` is intended to resolve bundled assemblies first, then
paths from `AIKERNEL_GOVERNANCE_ASSEMBLY_PATH`, then matching packages from the
NuGet global-packages cache.

`load_governance_runtime()` is intended to load the resolved assemblies through
pythonnet.

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

`backend` must provide the governance operations used by the client:

- `submit(request)`
- `snapshot(id)`
- `result(id)`

When pythonnet is available, wrappers can be converted to public C# DTOs with
`to_managed()`.

## Build

The 0.1.1.1 line validates and packages AIKernel.Control through NuGet only:

```powershell
cd C:\Users\HP\source\repos\AIKernel-NET\AIKernel.Control
dotnet test AIKernel.Control.slnx -c Release --no-restore
dotnet pack AIKernel.Control.slnx -c Release --no-restore -p:LocalPackageBuildNumber=3 -o ..\artifacts\local-packages
```

## Source Validation

Source-based Python installation is not a supported validation path for
AIKernel.Control 0.1.1.1. Keep Python experiments thin over the managed
assemblies and do not add CTG gate rules to Python code.
