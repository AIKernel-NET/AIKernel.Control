# AIKernel.Control Documentation

AIKernel.Control is the physical execution layer for AIKernel semantic graphs.
Use this documentation when you need to run, inspect, or package Control
engines for .NET and Python hosts.

## Start Here

- [User Guide](user-guide/index.md)
- [Architecture](architecture/index.md)
- [Bonsai mapping](bonsai-mapping/index.md)
- [Bonsai-1.7B built-in provider](bonsai-mapping/bonsai-1.7b-provider.md)
- [Execution engine](execution-engine/index.md)
- [Q1_0 CPU execution kernel](execution-engine/q1-0-cpu-kernel.md)
- [Control pipelines](pipelines/index.md)
- [Python governance wrapper](python/index.md)
- [Licensing](licensing/index.md)

## Operator Checklist

- Install the matching `AIKernel.Control.*` packages.
- Mount model assets through VFS/ROM instead of local ad hoc paths.
- Use CPU/Emulator packages for deterministic validation before binding GPU
  execution.
- Use the Python wrapper only as a thin managed assembly bridge.
