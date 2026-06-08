# aikernel-governance

[日本語](README-ja.md)

Python wrapper for the public governance surface of AIKernel.Control.

The stable package is published to PyPI:

```bash
pip install aikernel-governance
```

The distribution name is `aikernel-governance`. Import the module as
`aikernel_governance`.

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

The package does not provide a separate Python implementation of governance
semantics. It does not expose internal engine helpers, transport-specific logic,
OS-specific implementations, or private runtime state.

## Managed Assemblies

The wheel bundles the public Control and contract assemblies under
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

For source-based local validation, use a clean virtual environment:

```bash
pip install --force-reinstall \
  git+https://github.com/AIKernel-NET/AIKernel.Control.git#subdirectory=python
```
