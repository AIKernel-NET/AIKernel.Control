# AIKernel.Control Documentation

AIKernel.Control is the physical execution layer for AIKernel semantic graphs.
Use this documentation when you need to run, inspect, or package Control
engines for .NET hosts.

These docs describe Control as the AIOS SDK governance, security, and physical
execution layer. Control maps semantic graphs from Core onto explicit policies,
deterministic schedulers, diagnostics, and execution engines.

AIKernel.Monolith is the official AIOS distribution now in development. It is
planned as the standard reference distribution that integrates the control
plane with the Semantic OS layers after the 0.1.x line stabilizes.

## Cross-Repository Alignment

Shared repository boundaries, 0.1.1.1 local NuGet versioning, and the
NuGet-only / no-PyPI rule for this update line are defined by
[AIKernel Repository Alignment v0.1.1.1](https://github.com/AIKernel-NET/AIKernel.NET/blob/main/docs/development/repository-alignment-v0.1.1.1.md).

Control owns orchestration, policy application, Core gate invocation, runtime
control, and execution coordination. It must not reimplement Decision Gate or
Trajectory Gate truth tables, provider semantic evaluation, or browser runtime
execution.

## Start Here

- [User Guide](user-guide/index.md)
- [Architecture](architecture/index.md)
- [Bonsai mapping](bonsai-mapping/index.md)
- [Bonsai-1.7B built-in provider](bonsai-mapping/bonsai-1.7b-provider.md)
- [Execution engine](execution-engine/index.md)
- [Q1_0 CPU execution kernel](execution-engine/q1-0-cpu-kernel.md)
- [Control pipelines](pipelines/index.md)
- [CTG Control integration](development/control-ctg.md)
- [Concept Elevation Notes / 概念昇格ノート](development/concept-elevation.md)
- [Python governance wrapper](python/index.md)
- [Licensing](licensing/index.md)

## Which Page Should I Read?

- Read the User Guide when you want the install commands and the smallest
  deterministic emulator/CPU validation path.
- Read Architecture when you need to understand why semantic graphs are owned
  by Core while physical execution engines are owned here.
- Read Bonsai mapping before wiring Bonsai-style graphs to emulator, CPU, or
  GPU execution.
- Read CTG Control integration when you need to opt in to Core CTG gate
  evaluation during the Apply Policy stage.
- Read Python governance wrapper when you are consuming Control from Python and
  need to confirm that Python is only a managed assembly bridge. The 0.1.1.1
  line does not build or publish a PyPI package.

## First Validation

Start with CPU and Emulator validation before binding GPU execution:

```powershell
dotnet build AIKernel.Control.slnx -c Release
dotnet test AIKernel.Control.slnx -c Release --no-build
```

## Operator Checklist

- Install the matching `AIKernel.Control.*` packages.
- For 0.1.1.1 development, use NuGet packages only and local package versions
  such as `0.1.1.1-dev1`.
- Mount model assets through VFS/ROM instead of local ad hoc paths.
- Use CPU/Emulator packages for deterministic validation before binding GPU
  execution.
- Keep Python wrapper materials as reference-only unless a Python release is
  explicitly scheduled.
