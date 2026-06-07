# Execution Engine

AIKernel.Control is the execution-engine layer for AIKernel.

## Responsibilities

- Schedule semantic execution graphs onto CPU, GPU, or emulator backends.
- Preserve deterministic replay and step-by-step execution.
- Expose diagnostics for node timing, CPU/GPU load, breakpoints, watches, and
  traces.
- Keep physical execution separate from AIKernel.Core contracts and
  AIKernel.Demo samples.

## Runtime Analogy

AIKernel.Core is the semantic graph producer. AIKernel.Control is the runtime
that binds the graph to execution providers, similar to how ONNX Runtime binds
ONNX graphs to CPU/GPU execution providers.
