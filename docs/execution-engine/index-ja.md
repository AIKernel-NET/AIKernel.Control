# Execution Engine

[English](index.md)

AIKernel.Control は AIKernel の execution-engine layer です。

## 責務

- Semantic Execution Graph を CPU / GPU / Emulator backend へ schedule します。
- Deterministic replay と step-by-step execution を維持します。
- Node timing、CPU/GPU load、breakpoint、watch、trace の diagnostics を公開します。
- Physical execution を AIKernel.Core contracts と AIKernel.Demo samples から分離します。

## Runtime Analogy

AIKernel.Core は Semantic Graph Producer です。AIKernel.Control は、その graph を
execution provider へ bind する runtime です。

これは ONNX Runtime が ONNX graph を CPU/GPU execution provider へ bind する構造に
近いものです。

## Bonsai Execution

初期の標準モデル path は Bonsai-1.7B です。

- `BonsaiBuiltInProvider` は ROM loading、lifecycle observation、provider
  capability metadata を所有します。
- `Bonsai1BitCpuKernel` は Q1_0 CPU execution を所有します。
- `IBonsaiGpuExecutionDelegate` は CUDA、WebGPU、ROCm、Vulkan などの GPU
  delegation surface を定義します。

詳細は [Q1_0 CPU Execution Kernel](q1-0-cpu-kernel-ja.md) を参照してください。
