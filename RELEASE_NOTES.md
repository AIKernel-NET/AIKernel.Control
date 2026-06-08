# AIKernel.Control Release Notes

[日本語](RELEASE_NOTES-ja.md)

## 0.1.0

> [EN] Control 0.1.0 defines the public governance surface: execution requests, snapshots, and results become contract-stable.
>
> [JA] Control 0.1.0 は公開ガバナンス面を定義──Execution Request・Snapshot・Result が契約的に安定化される。

AIKernel.Control 0.1.0 introduces the physical execution layer for the AIKernel
semantic runtime.

- Consume public Control contracts from AIKernel.NET: execution graphs, nodes,
  requests, results, policies, schedulers, and state observers.
- Provide a deterministic ControlEmulator for CPU-only graph execution,
  breakpoints, watch, trace, and replay-oriented tests.
- Add the CPU-side Bonsai 1.7B built-in provider boundary with an allocation-free
  Q1_0 kernel and VFS/ROM model asset ownership.
- Keep GPU execution behind the Control.GPU boundary so device-specific
  execution can evolve without leaking into Core or Demo.
- Add the `aikernel-governance` Python package as a single wrapper over the
  public governance surface, including bundled managed assemblies and pythonnet
  loading.
- Document the responsibility split: Core owns semantic graphs, Control maps
  them to physical execution, and Demo only consumes the runtime.

Control 0.1.0 is the bridge from governed semantic graphs to deterministic
physical execution.
