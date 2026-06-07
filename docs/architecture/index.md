# AIKernel.Control Architecture

AIKernel.Control maps semantic execution graphs from AIKernel.Core onto physical
execution engines. Core remains the semantic runtime; Control owns scheduling,
emulation, CPU/GPU execution, and diagnostics.

## Layer Boundary

- `AIKernel.Control.Core` defines the control-plane abstractions.
- `AIKernel.Control.Emulator` converts Bonsai Graphs into AIKernel Graphs and
  executes them deterministically.
- `AIKernel.Control.CPU` maps Bonsai Nodes to CPU Operators.
- `AIKernel.Control.GPU` maps Bonsai Nodes to GPU kernels and tensor
  Capabilities.
- `AIKernel.Control.Diagnostics` observes graph execution, replay, timing, and
  load.

AIKernel.Demo must use Control; it must not own execution-engine code.
