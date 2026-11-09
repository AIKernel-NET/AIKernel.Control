# aikernel-governance

[日本語](README-ja.md)

Python wrapper surface for AIKernel.Control.

Starting with the 0.1.3 canon line, `aikernel-governance` is the PyPI package
for the public AIKernel.Control governance boundary. The package remains a thin
wrapper over managed C# assemblies and does not re-implement CTG Gate logic in
Python.

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

Python packaging resolves the public Control and contract assemblies under
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
`AIKERNEL_GOVERNANCE_ASSEMBLY_PATH`, then matching packages from the NuGet
global-packages cache.

`load_governance_runtime()` loads the resolved assemblies through pythonnet.

## Managed API Catalog

The v0.1.3 package exposes the generated managed API catalog through
`managed_api_catalog()`, `managed_api_summary()`, `managed_type_names()`, and
`find_managed_type(full_name)`.

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

## Build and Validate

For local validation:

```powershell
cd C:\Users\HP\source\repos\AIKernel-NET\AIKernel.Control
py -m pytest python\tests
```

## Distribution

PyPI publishing is handled by the repository GitHub Actions workflow on release
tags using Trusted Publishing. Keep Python experiments thin over the managed
assemblies and do not add CTG Gate rules to Python code.
