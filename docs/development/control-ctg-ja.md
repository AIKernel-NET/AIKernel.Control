# CTG Control 統合

[English](control-ctg.md)

このページは、AIKernel.NET が提供する Canonical Triadic Governance 契約と、
AIKernel.Core が評価する CTG evaluator を AIKernel.Control に opt-in 接続する
ための開発者向け説明です。

## Governance Model

CTG は contract-level の governance boundary として扱います。

- `Logos`、`Ethos`、`Pathos` は council vote として正規化します。
- `GateInput` は 3 つの離散的な `CouncilVoteValue` のみを保持します。
- Decision Gate / Trajectory Gate の評価は AIKernel.Core が所有します。
- AIKernel.Control は approve count、veto 条件、halt 集約を計算しません。
- Provider の confidence、risk、score carrier は Control の CTG vote-output surface
  では受け取りません。`ProviderVoteOutput` は discrete-only であり、`GateInput` は
  3 つの council vote のみを保持します。

Control における CTG は Apply Policy stage に属します。Policy は物理実行へ進むかを
決めますが、Gate decision 自体は Core evaluator の結果として扱います。

```text
Provider output
  -> CouncilVote
  -> CouncilDecision
  -> GateInput
  -> Core IDecisionGate
  -> DecisionGateResult
  -> ControlPolicyEvaluation
```

## Package Boundary

| Package | Responsibility |
| --- | --- |
| `AIKernel.NET` | Contract、DTO、enum、Canon/ROM carrier を保持します。 |
| `AIKernel.Core` | pure CTG evaluator と governance trace 構築を担当します。 |
| `AIKernel.Control.Core` | Provider vote 正規化、orchestration、policy adapter、DI 接続を担当します。 |
| `AIKernel.Control.Emulator` | policy 評価後の deterministic execution validation を担当します。 |
| `AIKernel.Control.CPU` / `AIKernel.Control.GPU` | 物理実行 kernel のみを担当します。CTG logic は置きません。 |

## Opt-In DI

`AllowAllControlPolicy` は emulator で使いやすい既定 policy として残します。CTG は
host が明示的に service 登録した場合だけ有効になります。

```csharp
using AIKernel.Control.Core.Ctg;
using AIKernel.Enums.Governance;

services.AddCtgControl(new CtgControlCoordinatorOptions
{
    ProviderOutputs =
    [
        new ProviderVoteOutput
        {
            ProviderId = "provider.logos",
            CouncilKind = CouncilKind.Logos,
            VoteValue = CouncilVoteValue.Approve
        },
        new ProviderVoteOutput
        {
            ProviderId = "provider.ethos",
            CouncilKind = CouncilKind.Ethos,
            VoteValue = CouncilVoteValue.Approve
        },
        new ProviderVoteOutput
        {
            ProviderId = "provider.pathos",
            CouncilKind = CouncilKind.Pathos,
            VoteValue = CouncilVoteValue.Abstain
        }
    ]
});
```

登録は `TryAdd` semantics を使うため、host が既に提供している service を上書きしません。

## Pipeline Objects

- `ProviderVoteAdapter` は `ProviderVoteOutput` を `CouncilVote` に変換します。
- `CtgCouncilEvaluationProviderRegistry` は local orchestration と test のために
  provider vote output を保持します。provider が見つからない場合は `Unknown` vote、
  同じ council に複数 provider が一致した場合は deterministic error にします。
- `CtgCouncilEvaluationOrchestrator` は semantic evaluation や gate evaluation を行わず、
  registry から provider vote output を解決します。
- `CouncilDecisionBuilder` は `CouncilEvaluationResult` を作成し、欠けた council を
  `Unknown` vote で補完します。
- `CouncilDecisionToGateInputAdapter` は `Logos`、`Ethos`、`Pathos` の vote value
  だけを抽出します。
- `CtgControlCoordinator` は Core `IDecisionGate` を呼び出し、
  `CtgControlDecisionEnvelope` を構築します。
- `CtgControlPolicyAdapter` は envelope を既存の `ControlPolicyEvaluation` に写像します。
- `CtgPolicyDecisionMapper` は accepted decision gate を `ALLOW`、rejected decision
  gate を `DENY`、rejected trajectory gate を `ABORT` に写像します。
- `AddCtgControl()` は decision / trajectory evaluation の両方が必要な host 向けに
  Core `IDecisionGate` と `ITrajectoryGate` 実装を登録します。

## Emulator Dry-Run

`AIKernel.Control.Emulator` は deterministic dry-run 用の CTG scenario helper を提供します。

- `CtgControlEmulator` は Control policy adapter と既存 emulator engine を通じて
  scenario を実行します。
- `CtgTruthTableScenario` は 27 個の離散 `Approve` / `Abstain` / `Reject`
  combination を作成します。
- `EthosRejectScenario`、`AllAbstainDenyScenario`、
  `EmptyTrajectoryHaltScenario`、`AnyDenyTrajectoryHaltScenario` は名前付きの
  regression scenario を提供します。
- `CtgReplayScenario` は replay check 用の安定した metadata を提供します。

Emulator は Core gate evaluator を呼び出します。CTG gate logic は保持しません。

## Diagnostics And Replay

`AIKernel.Control.Diagnostics` は formatting と replay helper を公開します。

- `CtgReplayMetadataWriter` は `StepGovernanceTrace` と `TrajectoryGateResult`
  metadata を serialize します。
- `CtgGovernanceTraceEmitter` は `IControlStateObserver` 経由で governance trace を
  emit します。
- `CtgDiagnosticsFormatter`、`CtgTraceRenderer`、formatter helper は開発 tool 向けの
  stable text view を提供します。
- `CtgGateTelemetryValidator` は gate metadata に provider confidence、risk、score
  carrier が含まれないことを検証します。

Reject reason kind は metadata では uppercase snake case で書き出し、C# enum 名は
PascalCase のまま維持します。

## Provider Boundary

Provider / model component は evidence と vote material を供給できますが、
`GateDecisionKind` や CTG execution decision を返してはいけません。
`BonsaiBuiltInProvider` は physical / model provider boundary のままであり、execution
result metadata に CTG gate field を含めません。

## Validation

通常の Control validation surface を実行します。

```powershell
dotnet restore AIKernel.Control.slnx
dotnet build AIKernel.Control.slnx -c Release --no-restore
dotnet test AIKernel.Control.slnx -c Release --no-build
```

CTG tests では allow、deny、halt mapping、Core との truth-table parity、replay metadata、
diagnostics emission、および provider diagnostics が `GateInput` carrier に入らないことを確認します。

## Current Limits

この接続には、opt-in policy adapter、deterministic emulator scenario、replay metadata
helper、diagnostics formatter が含まれます。Dynamic ProviderRouter integration は
0.1.1.1 surface には含めません。local orchestration では
`CtgCouncilEvaluationProviderRegistry` を使い、provider が見つからない場合は
`Unknown` vote、複数一致は deterministic error とします。

fallback routing は暗黙には行いません。後続で追加する場合も opt-in とし、Gate decision
ではなく vote material のみを返す必要があります。
