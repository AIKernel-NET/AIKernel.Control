using AIKernel.Core.Governance;
using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Creates a deterministic trajectory scenario containing a Core-denied step.
/// [EN] Documents this public package API member. [JA] Core により deny された step を含む決定論的 trajectory scenario を作成します。
/// </summary>
public sealed class AnyDenyTrajectoryHaltScenario
{
    private readonly CtgMockCouncilEvaluator _mockCouncilEvaluator;

    /// <summary>
    /// EN: Initializes a denied-step trajectory scenario factory.
    /// [EN] Documents this public package API member. [JA] denied-step trajectory scenario factory を初期化します。
    /// </summary>
    /// <param name="mockCouncilEvaluator">EN: The mock council evaluator. JA: mock council evaluator です。</param>
    public AnyDenyTrajectoryHaltScenario(CtgMockCouncilEvaluator? mockCouncilEvaluator = null)
    {
        _mockCouncilEvaluator = mockCouncilEvaluator ?? new CtgMockCouncilEvaluator();
    }

    /// <summary>
    /// EN: Creates the scenario by asking Core to evaluate the denied step.
    /// [EN] Documents this public package API member. [JA] Core に denied step を評価させて scenario を作成します。
    /// </summary>
    /// <returns>EN: The emulator scenario. JA: emulator scenario を返します。</returns>
    public CtgControlEmulatorScenario Create()
    {
        var decisionGate = new CtgDecisionGateEvaluator().Evaluate(
            new DecisionGateRequest
            {
                OperationId = "ctg.scenario.any_deny_trajectory",
                StepId = "ctg.denied.step",
                GateInput = new GateInput
                {
                    Logos = CouncilVoteValue.Unknown,
                    Ethos = CouncilVoteValue.Unknown,
                    Pathos = CouncilVoteValue.Unknown
                }
            });

        return _mockCouncilEvaluator.CreateScenario(
            "ctg.scenario.any_deny_trajectory",
            CouncilVoteValue.Approve,
            CouncilVoteValue.Approve,
            CouncilVoteValue.Abstain) with
        {
            TrajectorySteps =
            [
                new StepGovernanceTrace
                {
                    TraceId = "ctg.trace.any_deny_trajectory",
                    StepId = "ctg.denied.step",
                    DecisionGate = decisionGate,
                    RejectReasons = decisionGate.RejectReasons,
                    ObservedAt = decisionGate.ObservedAt
                }
            ]
        };
    }
}
