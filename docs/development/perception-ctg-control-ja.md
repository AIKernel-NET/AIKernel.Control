# Perception to CTG Control

`AIKernel.Control.Core.Perception` は、perception output と既存 CTG Control coordinator の間に opt-in adapter surface を追加します。

Control は perception を実行せず、Gate rule も実装しません。perception 由来の discrete vote candidate を `ProviderVoteOutput` に正規化し、既存 CTG coordinator に評価を委譲し、Core decision result を読んで pipeline carrier を選択します。

## 境界ルール

- approve-count、ethos-veto、trajectory halt logic は Control に実装しません。
- confidence、risk、score、diagnostics、explanation は `GateInput` の外側の metadata に留めます。
- dynamic pipeline selection は Core CTG output を読むだけで、Gate outcome を決めません。
- WASM runtime control、input injection、pause、halt は execution-side operation に留めます。

## v0.1.2 への整理

`PerceptionControlSignal` と `PerceptionControlRequest` は adapter-side carrier です。AIKernel.NET v0.1.2 で canonical perception/control contract が追加・統合された場合は置き換え可能です。
