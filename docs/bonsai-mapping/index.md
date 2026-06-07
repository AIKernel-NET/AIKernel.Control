# Bonsai Mapping

[日本語](index-ja.md)

AIKernel.Control treats Bonsai-style Behavior Tree and Graph Execution as a
physical execution model for AIKernel's semantic graphs.

## Mapping

- Bonsai Graph -> AIKernel Execution Graph.
- Bonsai Node -> CPU Operator or GPU Kernel.
- Bonsai edge/state transition -> deterministic AIKernel execution step.
- Bonsai trace/watch/breakpoint -> ReplayLog-compatible diagnostics.

ControlEmulator owns this mapping because it is an execution engine, not a demo
surface.

## Built-in Provider

The Bonsai-1.7B built-in provider maps `chat.local` and `text.tokenize` nodes to
the standard Bonsai inference boundary. Model metadata is mounted as ROM under
`/sys/roms/bonsai-1.7b/`; the provider never reads local files directly.

See [Bonsai-1.7B Built-in Provider](bonsai-1.7b-provider.md).
