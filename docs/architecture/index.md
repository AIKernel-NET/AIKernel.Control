# AIKernel.Control Architecture

[日本語](index-ja.md)

AIKernel.Control maps semantic execution graphs from AIKernel.Core onto physical
execution engines. Core remains the semantic runtime; Control owns scheduling,
emulation, CPU/GPU execution, and diagnostics.

## Layer Boundary

- `AIKernel.Control.Core` hosts the control-plane runtime entry points and
  consumes the shared control contracts from `AIKernel.Abstractions.Control`
  and `AIKernel.Dtos.Control`.
- `AIKernel.Control.Emulator` converts Bonsai Graphs into AIKernel Graphs and
  executes them deterministically.
- `AIKernel.Control.CPU` maps Bonsai Nodes to CPU Operators.
- `AIKernel.Control.GPU` maps Bonsai Nodes to GPU kernels and tensor
  Capabilities.
- `AIKernel.Control.Diagnostics` observes graph execution, replay, timing, and
  load.

AIKernel.Demo must use Control; it must not own execution-engine code.

## Python Boundary

The Python package `aikernel-governance` exposes the same public Control
boundary for Python hosts. It bundles the managed C# assemblies and wraps the
public governance surface through pythonnet.

Python sees the contract boundary: execution requests, results, snapshots,
provider metadata, Bonsai public wrappers, emulator wrappers, CPU kernel
wrappers, diagnostics wrappers, and the GPU delegate contract. It does not see
internal engine helpers, transport-specific implementation, or private runtime
state.

See [Python governance wrapper](../python/index.md).

## Standard Model Boundary

Bonsai-1.7B is exposed as a built-in Control provider, not as a demo fixture.
The provider uses VFS ROM paths for model assets and delegates physical
inference to CPU/GPU kernels through `IBonsaiInferenceKernel`.

This keeps the dependency direction intact:

- AIKernel.NET defines contracts.
- AIKernel.Control implements execution.
- AIKernel.Demo consumes execution.

## License Boundary

AIKernel.Control implementation code is Apache-2.0 licensed. The AIKernel.NET
contracts it consumes are MIT licensed. Third-party Bonsai, tokenizer, and
ggml/llama.cpp-derived assets keep their original licenses and are not
relicensed by this repository.
