using AIKernel.Enums.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Creates a deterministic scenario with no trajectory steps.
/// EN: Documentation for public API. JA: trajectory step がない決定論的 scenario を作成します。
/// </summary>
public sealed class EmptyTrajectoryHaltScenario
{
    private readonly CtgMockCouncilEvaluator _mockCouncilEvaluator;

    /// <summary>
    /// EN: Initializes an empty-trajectory scenario factory.
    /// EN: Documentation for public API. JA: empty-trajectory scenario factory を初期化します。
    /// </summary>
    /// <param name="mockCouncilEvaluator">EN: The mock council evaluator. JA: mock council evaluator です。</param>
    public EmptyTrajectoryHaltScenario(CtgMockCouncilEvaluator? mockCouncilEvaluator = null)
    {
        _mockCouncilEvaluator = mockCouncilEvaluator ?? new CtgMockCouncilEvaluator();
    }

    /// <summary>
    /// EN: Creates the scenario.
    /// EN: Documentation for public API. JA: scenario を作成します。
    /// </summary>
    /// <returns>EN: The emulator scenario. JA: emulator scenario を返します。</returns>
    public CtgControlEmulatorScenario Create()
        => _mockCouncilEvaluator.CreateScenario(
            "ctg.scenario.empty_trajectory",
            CouncilVoteValue.Approve,
            CouncilVoteValue.Approve,
            CouncilVoteValue.Abstain) with
        {
            TrajectorySteps = []
        };
}
