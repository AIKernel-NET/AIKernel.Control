# AIKernel.Control

[English README](README.md)

AIKernel.Core が生成する意味論的 Execution を、CPU / GPU / Emulator などの
物理実行へマッピングする Control Plane リポジトリです。

AIKernel.Control は AIKernel の物理実行レイヤーです。

- Semantic Graph（AIKernel.Core）を Physical Execution にマッピングします。
- CPU / GPU / Emulator の実行エンジンを提供します。
- Bonsai-1.7B の標準 Provider を内蔵します。

AIOS SDK において、AIKernel.Control は governance / security / physical
execution layer です。semantic graph を明示的な policy、決定論的 scheduler、
diagnostics、execution engine へ接続し、AIOS distribution の制御層を構成します。

AIKernel には、公式 AIOS ディストリビューションである **AIKernel.Monolith** もあります。
Monolith は 0.1.x 系の安定化後に、governance / control-plane layer を他の
SDK layer と統合する標準 AIOS として開発が開始されています。

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

AIKernel.Control 0.1.2 は AIKernel.Core 0.1.2 と同じ開発方針に従います。
NuGet package と同期した Python wrapper を公開し、local development package には
`0.1.2-dev{build-number}` を使います。

## クイックスタート

GPU や model asset を bind する前に、deterministic な Emulator と CPU package から
始めてください。Control は model weights を同梱しません。model asset は VFS / ROM
経由で mount します。

```bash
dotnet add package AIKernel.Control --version 0.1.2
```

host の依存面を小さくしたい場合のみ、split package を直接導入します。

```bash
dotnet add package AIKernel.Control.Core --version 0.1.2
dotnet add package AIKernel.Control.CPU --version 0.1.2
dotnet add package AIKernel.Control.Emulator --version 0.1.2
```

repository surface を検証します。

```powershell
dotnet build AIKernel.Control.slnx -c Release
dotnet test AIKernel.Control.slnx -c Release --no-build
```

`AIKernel.Control.GPU` は、CPU / Emulator validation が通り、GPU execution backend を
意図的に統合する段階で追加してください。

最初に作成する emulator object:

```csharp
using AIKernel.Control.Emulator;

var engine = new ControlEmulatorEngine(
    new DeterministicNodeScheduler(),
    new AllowAllControlPolicy());
```

これは Control の最小入口です。deterministic scheduling と明示的な policy boundary を
確認してから、CPU、diagnostics、GPU package へ進んでください。

## CTG Control Governance

AIKernel.Control は `Apply Policy` pipeline stage で Canonical Triadic Governance を
opt-in 接続できます。Control は CTG gate rule を所有しません。provider vote material を
`CouncilVote` / `CouncilDecision` へ正規化し、vote-only の `GateInput` を抽出し、
`IDecisionGate` / `ITrajectoryGate` 経由で AIKernel.Core へ評価を委譲します。

CTG Control surface は意図的に狭く保ちます。

- `GateInput` は `Logos`、`Ethos`、`Pathos` の 3 vote のみを保持します。
- Control、Diagnostics、Emulator は approve count、veto 条件、trajectory halt
  aggregation を計算しません。
- confidence、risk、score のような continuous provider carrier は Control CTG
  vote-output surface では受け取りません。
- Provider registry fallback は明示的です。provider が見つからない場合は `Unknown`
  vote、複数一致は deterministic error、fallback routing は opt-in のみに限定します。

[CTG Control integration](docs/development/control-ctg-ja.md) も参照してください。

## プロジェクト構成

- `AIKernel.Control` - Control Core、CPU、Emulator、Diagnostics、GPU boundary を
  導入する dependency-only の入口 package です。
- `AIKernel.Control.Core` - Control Plane の Runtime Entry Package です。
  共有契約は `AIKernel.Abstractions.Control` と `AIKernel.Dtos.Control` にあり、
  このプロジェクトはそれらを参照します。CPU / GPU / Emulator 実装が
  Interface や DTO を重複定義しないための境界です。
  Capability manifest は Control 固有の descriptor DTO ではなく、
  `AIKernel.Dtos.Capabilities.CapabilityModuleDescriptor` を使用します。CTG の
  provider vote adapter、coordinator、opt-in policy adapter、DI extension も
  提供します。
- `AIKernel.Control.Emulator` - ControlEmulator です。AIKernel `ExecutionGraph`
  を CPU-only Runtime で決定論的に実行します。Bonsai そのものを模倣する
  Emulator ではありません。step-by-step 実行、breakpoint、watch、trace、
  deterministic replay を扱います。Core evaluator の結果と比較する CTG dry-run
  scenario も含みますが、gate logic は複製しません。
- `AIKernel.Control.CPU` - Bonsai Node を CPU Operator へマッピングする
  CPU 実行エンジンです。SIMD / AVX 最適化、ThreadPool / TaskGraph 実行を
  担当します。
- `AIKernel.Control.GPU` - Bonsai Node を GPU Kernel へマッピングする
  GPU 実行エンジンです。Tensor Capability binding、GPU memory management、
  stream / event orchestration、graph execution を担当します。
- `AIKernel.Control.Diagnostics` - Observability レイヤーです。
  Graph visualization、node timing、CPU / GPU load inspection、ReplayLog
  integration、CTG trace / replay metadata formatting を担当します。

## Python Wrapper 参照

`aikernel-governance` は、AIKernel.Control の public governance surface を
Python から扱うために予約している wrapper 名です。

0.1.2 development line では 同期 Python wrapper を公開します。既存の
Python 関連資料は、参考および将来明示的に予定される Python release のために残します。

C# package の境界を、単一の Python API として公開します。

- execution request、result、snapshot envelope
- provider contract metadata
- Bonsai provider、tokenizer、model config、model-state wrapper
- emulator graph、node、scheduler、policy、engine wrapper
- CPU kernel と diagnostics wrapper
- managed assembly discovery と pythonnet loading

Python wrapper design は Control internals、scheduler logic、provider execution、
CPU / GPU kernel を Python で再実装しません。公開 managed contract の上に置く
薄い wrapper layer として扱います。

## Built-in Bonsai Model

`AIKernel.Control.Core` は、Bonsai-1.7B の標準 Control Plane Provider として
`BonsaiBuiltInProvider` を含みます。

この Provider は、`IVfsProvider` を通じて
`/sys/roms/bonsai-1.7b/` から `config.json` と `tokenizer.json` を読み込みます。
同じ ROM namespace に `model.q1_0.bin` が存在する場合は、それも任意で
バインドします。

初期化と実行の状態は `IControlStateObserver` を通じて、以下のような
決定論的な execution state snapshot として通知されます。

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

- [Documentation index](docs/README-ja.md)
- [User Guide](docs/user-guide/index-ja.md)
- [Architecture](docs/architecture/index-ja.md)
- [Bonsai mapping](docs/bonsai-mapping/index-ja.md)
- [Bonsai-1.7B built-in provider](docs/bonsai-mapping/bonsai-1.7b-provider-ja.md)
- [Execution engine](docs/execution-engine/index-ja.md)
- [Q1_0 CPU execution kernel](docs/execution-engine/q1-0-cpu-kernel-ja.md)
- [Control pipelines](docs/pipelines/index-ja.md)
- [CTG Control integration](docs/development/control-ctg-ja.md)
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

## パッケージインストール

.NET host では標準 Control surface の入口 package を使用します。

```bash
dotnet add package AIKernel.Control --version 0.1.2
```

依存面を小さくしたい host だけが split package を直接導入します。

```bash
dotnet add package AIKernel.Control.Core --version 0.1.2
dotnet add package AIKernel.Control.CPU --version 0.1.2
dotnet add package AIKernel.Control.Emulator --version 0.1.2
```

Python wrapper は 0.1.2 public package line に同期した
`aikernel-governance` として公開します。

0.1.2 の package scope と validation flow は
[Python governance wrapper](docs/python/index-ja.md) を参照してください。

## コントリビュータ向けガイドライン

Control の変更は、AIKernel 共通の開発規律に従ってください。

- [AIKernel 開発ガイドライン](../AIKernel.NET/docs/guidelines/AIKERNEL_DEVELOPMENT_GUIDELINES-jp.md)
- [AIKernel Development Guidelines](../AIKernel.NET/docs/guidelines/AIKERNEL_DEVELOPMENT_GUIDELINES.md)

Execution / governance code は fail-closed contract を公開し、Bonsai と emulator
behavior を deterministic に保ち、public boundary の外へ implementation exception
を漏らさず、Python wrapper を public C# surface と整合させてください。
