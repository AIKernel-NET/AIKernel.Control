# Bonsai Mapping

AIKernel.Control treats Bonsai-style Behavior Tree and Graph Execution as a
physical execution model for AIKernel's semantic graphs.

## Mapping

- Bonsai Graph -> AIKernel Execution Graph.
- Bonsai Node -> CPU Operator or GPU Kernel.
- Bonsai edge/state transition -> deterministic AIKernel execution step.
- Bonsai trace/watch/breakpoint -> ReplayLog-compatible diagnostics.

ControlEmulator owns this mapping because it is an execution engine, not a demo
surface.
