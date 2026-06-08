# Q1_0 CPU Execution Kernel

[日本語](q1-0-cpu-kernel-ja.md)

`Bonsai1BitCpuKernel` is the initial CPU backend for Bonsai Q1_0 inference. It
implements the 1-bit row layout used by ggml-style quantization and exposes
small validation surfaces for dequantization and dot-product checks.

## Block Layout

Each Q1_0 block contains 256 weights:

- 2 bytes: IEEE half-precision scale `d`
- 32 bytes: packed sign bits

This matches the ggml Q1_0 layout used in llama.cpp.

The packed bit value maps to a signed weight:

- `0` -> `-1`
- `1` -> `+1`

The dot-product rule is:

`d * sum(sign(qs_i) * x_i)`

Multiplication by the weight is therefore replaced with conditional add/subtract
over the input vector.

## Hot Path Rules

The inference hot path must remain allocation-clean:

- no boxing
- no LINQ
- no closures or captured delegates
- no direct file I/O
- no per-token heap allocation

The kernel is WebAssembly-compatible and avoids GC pressure entirely.

The implementation uses `Span<T>`/`ReadOnlySpan<T>` and vectorized accumulation
where supported. Validation helpers are explicit so tests can compare Q1_0
behavior without loading a full model ROM.

`DequantizeRowQ1_0` and `DotRowQ1_0` allow unit tests to validate Q1_0 behavior
without loading a full model.

## Backend Boundary

`IBonsaiInferenceKernel` is the execution boundary. The CPU backend implements
it directly. GPU backends should implement `IBonsaiGpuExecutionDelegate` and
preserve the same Q1_0 semantics, state binding, and deterministic output
requirements.

All backends must preserve deterministic output for identical inputs.

## Licensing

The C# kernel implementation is Apache-2.0 licensed as part of AIKernel.Control.

The Q1_0 format is documented for interoperability with ggml/llama.cpp-derived
assets. If an implementation copies or vendors third-party source files, it must
carry the corresponding third-party license and notice files in that downstream
repository or package.
