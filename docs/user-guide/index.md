# AIKernel.Control User Guide

This guide explains how to consume AIKernel.Control as the physical execution
layer for AIKernel semantic graphs.

Control is the AIOS SDK governance, security, and physical execution layer. It
lets a distribution bind semantic graphs to policies, schedulers, diagnostics,
CPU/emulator execution, and optional GPU execution.

AIKernel.Monolith is the official AIOS distribution now in development. It will
serve as the standard reference distribution that embodies semantic runtime,
capability graph, and governance after the 0.1.x line stabilizes.

## Installation

Install the Control packages that match your host role:

```bash
dotnet add package AIKernel.Control.Core --version 0.1.1.1
dotnet add package AIKernel.Control.CPU --version 0.1.1.1
dotnet add package AIKernel.Control.Emulator --version 0.1.1.1
dotnet add package AIKernel.Control.Diagnostics --version 0.1.1.1
```

Add `AIKernel.Control.GPU` only when the host binds a concrete GPU backend:

```bash
dotnet add package AIKernel.Control.GPU --version 0.1.1.1
```

Python wrapper materials are reference-only for the 0.1.1.1 update line. Do not
build, install, or publish a PyPI package unless a Python release is explicitly
scheduled.

## Runtime Roles

| Package | Role |
| --- | --- |
| `AIKernel.Control.Core` | Shared control-plane entry point and Bonsai provider contracts. |
| `AIKernel.Control.CPU` | Deterministic CPU execution kernel for validation and CPU hosts. |
| `AIKernel.Control.Emulator` | Step-by-step graph execution, replay, watches, and breakpoints. |
| `AIKernel.Control.Diagnostics` | Timing, graph, and replay inspection surfaces. |
| `AIKernel.Control.GPU` | GPU delegate boundary for concrete GPU execution backends. |

## Opt-In CTG Policy

Use CTG Control integration when the Apply Policy phase should call the Core
governance evaluators before physical execution. Register it explicitly:

```csharp
using AIKernel.Control.Core.Ctg;
using AIKernel.Enums.Governance;

services.AddCtgControl(new CtgControlCoordinatorOptions
{
    ProviderOutputs =
    [
        new ProviderVoteOutput
        {
            ProviderId = "provider.logos",
            CouncilKind = CouncilKind.Logos,
            VoteValue = CouncilVoteValue.Approve
        }
    ]
});
```

Provider vote material is discrete-only. Missing councils are normalized to
`Unknown`; multiple matching providers are deterministic errors. Control calls
Core gate evaluators and does not implement CTG Gate rules.

See [CTG Control integration](../development/control-ctg.md).

## Asset Mounting

Control does not vendor model weights or tokenizer assets. Mount assets through
the AIKernel VFS/ROM boundary so execution remains reproducible:

```text
/sys/roms/bonsai-1.7b/config.json
/sys/roms/bonsai-1.7b/tokenizer.json
/sys/roms/bonsai-1.7b/model.q1_0.bin
```

Use the emulator or CPU package to validate asset visibility before enabling a
GPU execution delegate.

## Python Wrapper

The archived Python package design exposes the public governance surface:

```python
from aikernel_governance import ExecutionRequest, GovernanceClient
```

It loads bundled managed assemblies and delegates semantics to the C# packages.
Do not treat the Python wrapper as an independent implementation.

For 0.1.1.1, do not build, install, or publish a PyPI package.

## Verification

Run the repository tests before publishing package updates:

```powershell
dotnet build AIKernel.Control.slnx -c Release -p:WarningsAsErrors=1591
dotnet test AIKernel.Control.slnx -c Release --no-build
dotnet pack AIKernel.Control.slnx -c Release --no-build --no-restore -p:UseLocalPackageVersion=true -p:LocalPackageBuildNumber=3 -o ..\artifacts\local-packages
```

## Failure Behavior

Control should fail closed when assets, graph contracts, or execution delegates
do not match. Prefer explicit diagnostics and replayable state over partial
success.
