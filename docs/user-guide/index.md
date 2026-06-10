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
dotnet add package AIKernel.Control.Core --version 0.1.1
dotnet add package AIKernel.Control.CPU --version 0.1.1
dotnet add package AIKernel.Control.Emulator --version 0.1.1
dotnet add package AIKernel.Control.Diagnostics --version 0.1.1
```

Add `AIKernel.Control.GPU` only when the host binds a concrete GPU backend:

```bash
dotnet add package AIKernel.Control.GPU --version 0.1.1
```

Python hosts use the single governance wrapper package:

```bash
pip install aikernel-governance
```

## Runtime Roles

| Package | Role |
| --- | --- |
| `AIKernel.Control.Core` | Shared control-plane entry point and Bonsai provider contracts. |
| `AIKernel.Control.CPU` | Deterministic CPU execution kernel for validation and CPU hosts. |
| `AIKernel.Control.Emulator` | Step-by-step graph execution, replay, watches, and breakpoints. |
| `AIKernel.Control.Diagnostics` | Timing, graph, and replay inspection surfaces. |
| `AIKernel.Control.GPU` | GPU delegate boundary for concrete GPU execution backends. |

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

## Verification

Run the repository tests before publishing package updates:

```powershell
dotnet build AIKernel.Control.slnx -c Release -p:WarningsAsErrors=1591
dotnet test AIKernel.Control.slnx -c Release --no-build
py -m pytest python/tests
```

## Failure Behavior

Control should fail closed when assets, graph contracts, or execution delegates
do not match. Prefer explicit diagnostics and replayable state over partial
success.
