# Perception to CTG Control

`AIKernel.Control.Core.Perception` は、perception output と既存 CTG Control coordinator の間に opt-in adapter surface を追加します。

Control は perception を実行せず、Gate rule も実装しません。perception 由来の discrete vote candidate を `ProviderVoteOutput` に正規化し、既存 CTG coordinator に評価を委譲し、Core decision result を読んで pipeline carrier を選択します。

## 境界ルール

- approve-count、ethos-veto、trajectory halt logic は Control に実装しません。
- confidence、risk、score、diagnostics、explanation は `GateInput` の外側の metadata に留めます。
- dynamic pipeline selection は Core CTG output を読むだけで、Gate outcome を決めません。
- WASM runtime control、input injection、pause、halt は execution-side operation に留めます。
- `health-death` retry intent のような高優先 sensor carrier は `GateInput` の外側に留めます。
  Control は Core Gate result が得られた後、`CtgControlExecutionContext` と
  `CtgControlDecisionEnvelope` metadata に carrier として付与できます。
- `PerceptionPipelineSelector` は付与された retry intent carrier から `retry` を選択できます。
  これは pipeline routing であり Gate 評価ではありません。Core decision result は変更せず、
  envelope 内に観測可能なまま残します。

## Sensor OS input

Control は Sensor OS output を adapter-side carrier として受け取ります。
visual、auditory、health、compass、spatial sensor の実行は行いません。

| Sensor | Concept | Control の扱い |
| --- | --- | --- |
| `visual`, `audio`, `health` | `Aisthesis` | `GateInput` の外側の metadata または retry intent に留める。 |
| `motor` | `Kinesis` | policy 後の runtime dispatch へ使えるが、Gate 評価には使わない。 |
| `compass`, `spatial` | `Phantasia` | pipeline selection へ使えるが、Gate decision には使わない。 |

repository ownership と昇格ルールは
[リポジトリ横断開発者ガイド v0.1.1.1](https://github.com/AIKernel-NET/AIKernel.NET/blob/main/docs/development/cross-repository-developer-guide-v0.1.1.1-ja.md)
を参照してください。

## v0.1.3 への整理

`PerceptionControlSignal`、`PerceptionControlRequest`、`PerceptionPipelineSelection`、retry intent carrier は adapter-side carrier です。AIKernel.NET v0.1.3 で canonical perception/control contract が追加・統合された場合は置き換え可能です。
