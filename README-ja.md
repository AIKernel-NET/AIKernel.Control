# AIKernel.Control

[English README](README.md)

AIKernel.Core が生成する意味論的 Execution を、CPU / GPU / Emulator などの
物理実行へマッピングする Control Plane リポジトリです。

AIKernel.Control は AIKernel の物理実行レイヤーです。

- Semantic Graph（AIKernel.Core）を Physical Execution にマッピングします。
- CPU / GPU / Emulator の実行エンジンを提供します。
- Bonsai-1.7B の標準 Provider を内蔵します。

## リポジトリの役割

AIKernel.Control は、AIKernel の Execution Engine ワークスペースです。
AIKernel.Core は意味論的グラフと決定論的 Runtime Contract を所有し、
AIKernel.Control はそれらを物理実行エンジン、スケジューラ境界、
診断、Bonsai スタイルの Graph Execution へマッピングします。

Bonsai スタイルとは、明示的な operator と決定論的 scheduling を持つ
node-based execution graph を指します。

Demo リポジトリは利用者側です。Control は実行エンジンを所有します。
Emulator や CPU / GPU Scheduler を Control に置くことで、Demo コードが
Runtime 依存になることを防ぎます。

Control は AIKernel.Demo に依存しません。Demo は Control を利用する側であり、
Control は独立した Runtime Surface として動作します。

AIKernel.Control は、2026-06-09 に予定している 0.1.0 Prototype Validation
Phase に参加します。AIKernel の Semantic Graph を物理実行エンジンへつなぐ
経路を検証しつつ、その責務を AIKernel.Demo へ移さないことを保証します。

## プロジェクト構成

- `AIKernel.Control.Core` - Control Plane の Runtime Entry Package です。
  共有契約は `AIKernel.Abstractions.Control` と `AIKernel.Dtos.Control` にあり、
  このプロジェクトはそれらを参照します。CPU / GPU / Emulator 実装が
  Interface や DTO を重複定義しないための境界です。
  Capability manifest は Control 固有の descriptor DTO ではなく、
  `AIKernel.Dtos.Capabilities.CapabilityModuleDescriptor` を使用します。
- `AIKernel.Control.Emulator` - ControlEmulator です。AIKernel `ExecutionGraph`
  を CPU-only Runtime で決定論的に実行します。Bonsai そのものを模倣する
  Emulator ではありません。step-by-step 実行、breakpoint、watch、trace、
  deterministic replay を扱います。
- `AIKernel.Control.CPU` - Bonsai Node を CPU Operator へマッピングする
  CPU 実行エンジンです。SIMD / AVX 最適化、ThreadPool / TaskGraph 実行を
  担当します。
- `AIKernel.Control.GPU` - Bonsai Node を GPU Kernel へマッピングする
  GPU 実行エンジンです。Tensor Capability binding、GPU memory management、
  stream / event orchestration、graph execution を担当します。
- `AIKernel.Control.Diagnostics` - Observability レイヤーです。
  Graph visualization、node timing、CPU / GPU load inspection、ReplayLog
  integration を担当します。

## Built-in Bonsai Model

`AIKernel.Control.Core` は、Bonsai-1.7B の標準 Control Plane Provider として
`BonsaiBuiltInProvider` を含みます。

この Provider は、`IVfsProvider` を通じて
`/sys/roms/bonsai-1.7b/` から `config.json` と `tokenizer.json` を読み込みます。
同じ ROM namespace に `model.q1_0.bin` が存在する場合は、それも任意で
バインドします。

初期化と実行の状態は `IControlStateObserver` を通じて、以下のような
決定論的 Phase Snapshot として通知されます。

- `ModelDownload`
- `Initializing`
- `Generating`

物理推論は `IBonsaiInferenceKernel` へ委譲されます。

`AIKernel.Control.CPU` は `Bonsai1BitCpuKernel` を提供します。これは Q1_0
1-bit 実行カーネルで、packed sign を `Span<T>` 入力に対する条件付き
加減算として評価します。推論ループ内の allocation を避け、ggml /
llama.cpp の量子化資産と検証できるように `DequantizeRowQ1_0` と
`DotRowQ1_0` を明示的に公開しています。

CPU kernel は allocation-free かつ WebAssembly-compatible であり、browser-side
execution を可能にします。

`AIKernel.Control.GPU` は `IBonsaiGpuExecutionDelegate` を公開します。
CUDA、WebGPU、ROCm、Vulkan などの GPU backend は、この同じ Bonsai inference
contract を実装できます。これにより `AIKernel.Control.Core` は具体的な GPU
Runtime に依存しません。

CPU / GPU プロジェクトは、`BonsaiBuiltInProvider` の物理実行部分のみを
担当します。モデル構造、Tokenizer parsing、Config parsing、Provider
lifecycle は `AIKernel.Control.Core` に残ります。

Control はモデル重みやローカルモデルファイルを所有しません。すべての
モデル資産は VFS / ROM にあり、Control は契約を通じてそれを読み取り、
バインド済みの model state を実行します。

関連ドキュメント:

- [Architecture](docs/architecture/index-ja.md)
- [Bonsai mapping](docs/bonsai-mapping/index-ja.md)
- [Bonsai-1.7B built-in provider](docs/bonsai-mapping/bonsai-1.7b-provider-ja.md)
- [Execution engine](docs/execution-engine/index-ja.md)
- [Q1_0 CPU execution kernel](docs/execution-engine/q1-0-cpu-kernel-ja.md)
- [Control pipelines](docs/pipelines/index-ja.md)
- [Licensing](docs/licensing/index-ja.md)

## 設計方針

ControlEmulator は AIKernel における ONNX Runtime のような位置づけです。
AIKernel.Core が Semantic Graph を生成し、Control がそれを物理実行 Provider
へバインドします。

この構造により、AIKernel.Control は AIKernel Semantic Runtime における
OSS 指向の Bonsai execution layer になります。

Control は AIKernel Semantic Runtime の物理実行レイヤーです。

## ライセンス

AIKernel.Control は Apache License 2.0 で提供されます。Control は
Execution Engine code と Physical Runtime Mapping を含むため、AIKernel.Core
や Native Capability Repository と同じく、特許条項を持つ Apache 2.0 を
採用します。

このリポジトリが利用する共有 Interface / DTO Package、たとえば
`AIKernel.Abstractions.*`、`AIKernel.Dtos.*`、`AIKernel.Enums.*` は
AIKernel.NET の一部であり、契約のみを含むため MIT License です。

Bonsai model files、tokenizer assets、llama.cpp / ggml 由来の quantization
references、およびその他の third-party ROM assets は、AIKernel.Control によって
再ライセンスされません。利用者は、それらの資産を元のライセンスと
再配布条件に従って使用する必要があります。

このリポジトリは VFS path と実行契約を文書化しますが、model weights や
ローカル環境パスは同梱しません。

## ビルド

```powershell
dotnet build AIKernel.Control.slnx
```

共通のプロジェクトプロパティは `Directory.Build.props` に集約されています。

0.1.0 prototype development 中は、NuGet cache collision を避けるために
`AIKernelPackageVersion` が `0.1.0.2` のようなローカルビルドを指す場合が
あります。Public release build では、package family を固定版の `0.1.0`
release version に揃えてください。
