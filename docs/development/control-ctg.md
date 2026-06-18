# CTG Control Integration

[日本語](control-ctg-ja.md)

This page describes the opt-in connection between AIKernel.Control and the
Canonical Triadic Governance contracts provided by AIKernel.NET and evaluated
by AIKernel.Core.

## Governance Model

CTG is treated as a contract-level governance boundary:

- `Logos`, `Ethos`, and `Pathos` are normalized as council votes.
- `GateInput` carries only the three discrete `CouncilVoteValue` values.
- Decision Gate and Trajectory Gate evaluation are owned by AIKernel.Core.
- AIKernel.Control never computes approve counts, veto conditions, or halt
  aggregation.
- Provider confidence, risk, and score carriers are not accepted by the Control
  CTG vote-output surface. `ProviderVoteOutput` is discrete-only, and
  `GateInput` remains only the three council votes.

In Control, CTG belongs to the Apply Policy stage. The policy decides whether
physical execution proceeds, but the gate decision itself remains a Core
evaluator result.

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
| `AIKernel.NET` | Contracts, DTOs, enums, and Canon/ROM contract carriers. |
| `AIKernel.Core` | Pure CTG evaluators and governance trace construction. |
| `AIKernel.Control.Core` | Provider vote normalization, orchestration, policy adapter, and DI connection. |
| `AIKernel.Control.Emulator` | Deterministic execution validation after policy evaluation. |
| `AIKernel.Control.CPU` / `AIKernel.Control.GPU` | Physical execution kernels only. CTG logic is not placed here. |

## Opt-In DI

`AllowAllControlPolicy` remains the default emulator-friendly policy. CTG is
enabled only when a host registers the CTG services:

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

The registration uses `TryAdd` semantics, so existing host-provided services are
not overwritten.

## Pipeline Objects

- `ProviderVoteAdapter` converts `ProviderVoteOutput` into `CouncilVote`.
- `CtgCouncilEvaluationProviderRegistry` stores provider vote outputs for local
  orchestration and tests. Missing providers become `Unknown` votes; multiple
  providers for the same council produce a deterministic error.
- `CtgCouncilEvaluationOrchestrator` resolves provider vote outputs from the
  registry without performing semantic or gate evaluation.
- `CouncilDecisionBuilder` creates `CouncilEvaluationResult` and fills missing
  councils with `Unknown` votes.
- `CouncilDecisionToGateInputAdapter` extracts only `Logos`, `Ethos`, and
  `Pathos` vote values.
- `CtgControlCoordinator` calls Core `IDecisionGate` and builds a
  `CtgControlDecisionEnvelope`.
- `CtgControlPolicyAdapter` maps the envelope into the existing
  `ControlPolicyEvaluation`.
- `CtgPolicyDecisionMapper` maps accepted decision gates to `ALLOW`, rejected
  decision gates to `DENY`, and rejected trajectory gates to `ABORT`.
- `AddCtgControl()` registers Core `IDecisionGate` and `ITrajectoryGate`
  implementations for hosts that need both decision and trajectory evaluation.

## Emulator Dry-Run

`AIKernel.Control.Emulator` provides CTG scenario helpers for deterministic
dry-runs:

- `CtgControlEmulator` runs a scenario through the Control policy adapter and
  the existing emulator engine.
- `CtgTruthTableScenario` creates the 27 discrete `Approve` / `Abstain` /
  `Reject` combinations.
- `EthosRejectScenario`, `AllAbstainDenyScenario`,
  `EmptyTrajectoryHaltScenario`, and `AnyDenyTrajectoryHaltScenario` provide
  named regression scenarios.
- `CtgReplayScenario` supplies stable metadata for replay checks.

The emulator calls Core gate evaluators. It does not contain CTG gate logic.

## Diagnostics And Replay

`AIKernel.Control.Diagnostics` exposes formatting and replay helpers:

- `CtgReplayMetadataWriter` serializes `StepGovernanceTrace` and
  `TrajectoryGateResult` metadata.
- `CtgGovernanceTraceEmitter` emits governance traces through
  `IControlStateObserver`.
- `CtgDiagnosticsFormatter`, `CtgTraceRenderer`, and formatter helpers provide
  stable text views for development tooling.
- `CtgGateTelemetryValidator` verifies that gate metadata does not contain
  provider confidence, risk, or score carriers.

Reject reason kinds are written in uppercase snake case metadata form, while
C# enum names remain PascalCase.

## Provider Boundary

Provider and model components may supply evidence and vote material, but they
must not return `GateDecisionKind` or CTG execution decisions. `BonsaiBuiltInProvider`
remains a physical/model provider boundary; its execution result metadata does
not contain CTG gate fields.

## Validation

Run the normal Control validation surface:

```powershell
dotnet restore AIKernel.Control.slnx
dotnet build AIKernel.Control.slnx -c Release --no-restore
dotnet test AIKernel.Control.slnx -c Release --no-build
```

The CTG tests cover allow, deny, halt mapping, truth-table parity with Core,
replay metadata, diagnostics emission, and the guarantee that provider
diagnostics do not enter the `GateInput` carrier.

## Current Limits

This connection includes the opt-in policy adapter, deterministic emulator
scenarios, replay metadata helpers, and diagnostics formatters. Dynamic
ProviderRouter integration remains intentionally outside the 0.1.1.1 surface:
local orchestration uses `CtgCouncilEvaluationProviderRegistry`, where missing
providers become `Unknown` votes and multiple matches are deterministic errors.

Fallback routing is not implicit. If it is added later, it must be opt-in and
must still return vote material rather than Gate decisions.
