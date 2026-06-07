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
