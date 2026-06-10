# AIKernel.Control Release Notes

[日本語](RELEASE_NOTES-ja.md)

## 0.1.1

**June 10th, 2026 - Governing the control plane.**
**2026年6月10日--制御プレーンを統治する。**

Governing the control plane: policies, schedulers, and emulators align into a
deterministic governance layer. 制御プレーンの統治--ポリシー・スケジューラ・
エミュレータが決定論的ガバナンス層へ整列する。

AIKernel.Control 0.1.1 synchronizes the physical execution and governance layer
for the AIKernel Semantic OS package family.

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

Control 0.1.1 is the governed bridge from semantic graphs to deterministic
physical execution.
