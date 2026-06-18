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

Install the entry package for the standard Control surface:

```bash
dotnet add package AIKernel.Control --version 0.1.2
```

Install split packages directly when your host intentionally wants a smaller
dependency surface:

```bash
dotnet add package AIKernel.Control.Core --version 0.1.2
dotnet add package AIKernel.Control.CPU --version 0.1.2
dotnet add package AIKernel.Control.Emulator --version 0.1.2
dotnet add package AIKernel.Control.Diagnostics --version 0.1.2
```

Add `AIKernel.Control.GPU` only when the host binds a concrete GPU backend:

```bash
dotnet add package AIKernel.Control.GPU --version 0.1.2
```

During local integration, use `0.1.2-dev{buildNumber}` NuGet packages instead
of stable `0.1.2` packages until the release task opens publication. Python
validation uses the `aikernel-governance` wheel with version
`0.1.2.dev{buildNumber}`.

## Runtime Roles

| Package | Role |
| --- | --- |
| `AIKernel.Control` | Dependency-only entry package for the standard Control surface. |
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

The Python package exposes the public governance surface:

```python
from aikernel_governance import ExecutionRequest, GovernanceClient
```

It loads bundled managed assemblies and delegates semantics to the C# packages.
Do not treat the Python wrapper as an independent implementation.

Do not treat the Python wrapper as an independent implementation. It exposes
managed loading helpers and the generated managed API catalog, and remains thin
over the C# packages.

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
