# Q1_0 CPU Execution Kernel

[English](q1-0-cpu-kernel.md)

`Bonsai1BitCpuKernel` は Bonsai Q1_0 inference の初期 CPU backend です。
ggml スタイルの quantization で使われる 1-bit row layout を実装し、
dequantization と dot-product 検証のための小さな surface を公開します。

## Block Layout

各 Q1_0 block は 256 個の weight を含みます。

- 2 bytes: IEEE half-precision scale `d`
- 32 bytes: packed sign bits

これは llama.cpp で使われる ggml Q1_0 layout と一致します。

packed bit は次の signed weight に対応します。

- `0` -> `-1`
- `1` -> `+1`

dot-product rule:

`d * sum(sign(qs_i) * x_i)`

weight による乗算は、input vector に対する条件付き add / subtract に置き換えられます。

## Hot Path Rules

Inference hot path は allocation-clean である必要があります。

- boxing なし
- LINQ なし
- closure / captured delegate なし
- direct file I/O なし
- per-token heap allocation なし

kernel は WebAssembly-compatible であり、GC pressure を完全に避けます。

実装は `Span<T>` / `ReadOnlySpan<T>` を使い、対応環境では vectorized accumulation を
行います。Validation helper は明示的に公開されているため、full model ROM を
読み込まなくても Q1_0 behavior を test できます。

`DequantizeRowQ1_0` と `DotRowQ1_0` により、full model を読み込まずに unit test で
Q1_0 behavior を検証できます。

## Backend Boundary

`IBonsaiInferenceKernel` が execution boundary です。CPU backend はこれを直接
実装します。

GPU backend は `IBonsaiGpuExecutionDelegate` を実装し、同じ Q1_0 semantics、
state binding、deterministic output requirements を維持してください。

すべての backend は、同一 input に対して deterministic output を維持する必要があります。

## Licensing

C# kernel implementation は AIKernel.Control の一部として Apache-2.0 です。

Q1_0 format は ggml / llama.cpp 由来 asset との interoperability のために
文書化されています。third-party source file を copy / vendor する実装では、
対応する third-party license と notice file を downstream repository または
package に含める必要があります。
