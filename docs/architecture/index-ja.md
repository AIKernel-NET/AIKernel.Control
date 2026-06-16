# AIKernel.Control アーキテクチャ

[English](index.md)

AIKernel.Control は、AIKernel.Core が生成する意味論的 Execution Graph を
物理実行エンジンへマッピングします。Core は Semantic Runtime の責務を持ち、
Control は scheduling、emulation、CPU/GPU execution、diagnostics を所有します。

## レイヤー境界

- `AIKernel.Control.Core` は Control Plane の Runtime Entry Point を提供し、
  `AIKernel.Abstractions.Control` と `AIKernel.Dtos.Control` にある共有契約を
  利用します。CTG orchestration と adapter もここに置きますが、CTG gate rule は
  ここに置きません。
- `AIKernel.Control.Emulator` は AIKernel `ExecutionGraph` を CPU-only で
  決定論的に実行します。CTG scenario は Core evaluator の結果と比較するだけで、
  decision をローカル実装しません。
- `AIKernel.Control.CPU` は Bonsai Node を CPU Operator へマッピングします。
- `AIKernel.Control.GPU` は Bonsai Node を GPU Kernel や Tensor Capability へ
  マッピングします。
- `AIKernel.Control.Diagnostics` は graph execution、replay、timing、load を
  観測します。CTG diagnostics は trace を整形・emit しますが、結果を判断しません。

AIKernel.Demo は Control を利用する側です。Demo が execution-engine code を
所有してはいけません。

## CTG Governance Boundary

Canonical Triadic Governance は Control の `Apply Policy` stage に接続します。
Control の責務は orchestration です。

- provider vote material を決定論的に解決します。
- material を `CouncilVote` と `CouncilDecision` に正規化します。
- vote-only の `GateInput` を抽出します。
- Core `IDecisionGate` / `ITrajectoryGate` を呼び出します。
- Core の結果を既存の Control policy result と replay metadata に写像します。

Control は Gate semantics を担当しません。approve count、Ethos veto behavior、
trajectory halt aggregation、Gate decision の生成を Control 内で行ってはいけません。

Provider routing は fail-closed かつ決定論的に扱います。provider が見つからない場合は
`Unknown` vote、複数一致は deterministic error、fallback routing は明示的な opt-in のみに限定します。

## Python 境界

`aikernel-governance` は、0.1.2 正典シリーズで同じ public Control boundary を
Python host 向けに公開する wrapper 名です。Python packaging は managed C#
assemblies と public governance surface を pythonnet 経由で wrapper します。
CTG、policy、emulator、scheduling logic を Python で再実装しません。

安定版 Python package は 0.1.2 公開タスクが開始された後に作成します。local
validation では、managed `0.1.2-dev<build-number>` package family と一致する
`0.1.2.dev<build-number>` wheel を使います。

Python から見えるのは契約境界です。execution request、result、snapshot、
provider metadata、Bonsai public wrappers、emulator wrappers、CPU kernel
wrappers、diagnostics wrappers、GPU delegate contract を扱えます。一方で、
internal engine helper、transport-specific implementation、private runtime
state は公開しません。

[Python governance wrapper](../python/index-ja.md) も参照してください。

## 標準モデル境界

Bonsai-1.7B は Demo fixture ではなく、Control の built-in Provider として
公開されます。

Provider は VFS ROM path から model assets を読み込み、
`IBonsaiInferenceKernel` を通じて CPU/GPU kernel へ物理推論を委譲します。

この構造により、依存方向は次のように保たれます。

- AIKernel.NET が契約を定義します。
- AIKernel.Control が実行を実装します。
- AIKernel.Demo が実行を利用します。

## ライセンス境界

AIKernel.Control の実装コードは Apache-2.0 です。利用する AIKernel.NET の
契約パッケージは MIT です。

Bonsai、tokenizer、ggml / llama.cpp 由来 asset は、それぞれの元ライセンスを
維持します。AIKernel.Control は third-party asset を再ライセンスしません。
