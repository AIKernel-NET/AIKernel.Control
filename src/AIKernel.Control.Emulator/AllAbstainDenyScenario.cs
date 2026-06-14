using AIKernel.Enums.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Creates a deterministic scenario where all councils abstain.
/// JA: すべての council が abstain する決定論的 scenario を作成します。
/// </summary>
public sealed class AllAbstainDenyScenario
{
    private readonly CtgMockCouncilEvaluator _mockCouncilEvaluator;

    /// <summary>
    /// EN: Initializes an all-abstain scenario factory.
    /// JA: all-abstain scenario factory を初期化します。
    /// </summary>
    /// <param name="mockCouncilEvaluator">EN: The mock council evaluator. JA: mock council evaluator です。</param>
    public AllAbstainDenyScenario(CtgMockCouncilEvaluator? mockCouncilEvaluator = null)
    {
        _mockCouncilEvaluator = mockCouncilEvaluator ?? new CtgMockCouncilEvaluator();
    }

    /// <summary>
    /// EN: Creates the scenario.
    /// JA: scenario を作成します。
    /// </summary>
    /// <returns>EN: The emulator scenario. JA: emulator scenario を返します。</returns>
    public CtgControlEmulatorScenario Create()
        => _mockCouncilEvaluator.CreateScenario(
            "ctg.scenario.all_abstain",
            CouncilVoteValue.Abstain,
            CouncilVoteValue.Abstain,
            CouncilVoteValue.Abstain);
}
