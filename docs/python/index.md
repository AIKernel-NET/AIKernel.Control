# Python Governance Wrapper

[日本語](index-ja.md)

`aikernel-governance` is the Python distribution for the public AIKernel.Control
governance surface in the v0.1.2 canonical series.

The wrapper sits over the C# packages; it is not a Python reimplementation of
Control or CTG. It exposes a single import surface:

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

Stable install, after publication opens:

```bash
pip install aikernel-governance==0.1.2
```

Local validation uses `0.1.2.dev{buildNumber}` wheels. Do not create stable
`0.1.2` artifacts until the release task explicitly requests them.

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
- generated managed API catalog helpers

It does not expose internal governance engine helpers, transport-specific code,
OS-specific implementations, private runtime internals, or CTG Gate rules.

## Managed Assemblies

The wheel bundles or resolves the Control and contract assemblies under
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

## Managed API Catalog

The v0.1.2 package exposes the generated managed API catalog through
`managed_api_catalog()`, `managed_api_summary()`, `managed_type_names()`, and
`find_managed_type(full_name)`.

## Build

```powershell
py -m compileall python\src python\tests
py -m pytest python\tests
py -m build --wheel
```

PyPI publication uses GitHub Actions Trusted Publishing and the `pypi`
environment.
## Trusted Publisher Configuration

The PyPI Trusted Publisher for the aikernel-governance project must match the GitHub OIDC claims emitted by this repository:

| Field | Value |
| --- | --- |
| PyPI project | aikernel-governance |
| Owner | AIKernel-NET |
| Repository | AIKernel.Control |
| Workflow | publish-pypi.yml |
| Environment | pypi |

If PyPI reports `invalid-publisher`, do not change the workflow to token credentials. Fix the PyPI project Trusted Publisher entry so it matches the table above, then rerun the failed publish job.
