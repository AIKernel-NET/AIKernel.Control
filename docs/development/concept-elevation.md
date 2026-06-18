# Concept Elevation Notes / 概念昇格ノート

## Canonical Reference / 正典参照

Common naming rules are maintained in AIKernel.NET:

- `AIKernel.NET/docs/canonical-language/index.md`
- `AIKernel.NET/docs/design/concept-elevation-refactoring-design.md`
- `AIKernel.NET/docs/guidelines/concept-elevation-guidelines.md`
- `AIKernel.NET/docs/migration/concept-elevation-v0.1.1.1.md`
- `AIKernel.NET/docs/todo/concept-elevation-refactoring-todo.md`

共通の concept dictionary と naming guideline は AIKernel.NET 側を正典として
参照します。

## Control Scope / Control の対象範囲

AIKernel.Control owns orchestration and policy application. It must not
reimplement CTG gate logic. Concept facades describe orchestration concepts
only and call no gate rules.

AIKernel.Control は orchestration と policy application を担当します。CTG gate
logic は再実装せず、concept facade は orchestration concept の説明に限定します。

Added concept surfaces:

- `AIKernel.Control.Core.Concepts.KairosTrigger`
- `AIKernel.Control.Core.Concepts.KairosScheduler`
- `AIKernel.Control.Core.Concepts.NousSupervisor`
- `AIKernel.Control.Core.Concepts.NousStrategy`

## Guardrails / 境界ルール

- Control does not calculate approve count, Ethos veto, or trajectory halt.
- Control does not create `GateDecisionKind`.
- Provider diagnostics do not flow into `GateInput`.
- Concept names do not appear on DTO, mapper, adapter, provider, or low-level
  execution engine names.

Control は approve count / Ethos veto / trajectory halt を計算しません。
`GateDecisionKind` を生成せず、provider diagnostics を `GateInput` に流しません。

`EthosRejectScenario` remains as an existing emulator compatibility name. It is
documented as a compatibility exception and must not be used as precedent for
new low-level Control type names.

`EthosRejectScenario` は既存 emulator compatibility 名として維持します。これは
互換例外であり、新規の低レイヤ Control 型名の前例にはしません。

## Tests / テスト

`tests/AIKernel.Control.Tests/ConceptElevationArchitectureTests.cs` guards the
naming boundary and verifies that concept facades stay deterministic without
duplicating Gate logic.

## Remaining Work / 残タスク

- Optional internal adoption must continue to call Core evaluators.
- Any CLI, diagnostics, or emulator display should show Core results rather
  than duplicating the rules.
