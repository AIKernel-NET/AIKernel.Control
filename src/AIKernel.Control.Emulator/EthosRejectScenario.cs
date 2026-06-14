using AIKernel.Enums.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Creates a deterministic scenario with an Ethos reject vote.
/// JA: Ethos reject vote を持つ決定論的 scenario を作成します。
/// </summary>
public sealed class EthosRejectScenario
{
    private readonly CtgMockCouncilEvaluator _mockCouncilEvaluator;

    /// <summary>
    /// EN: Initializes an Ethos reject scenario factory.
    /// JA: Ethos reject scenario factory を初期化します。
    /// </summary>
    /// <param name="mockCouncilEvaluator">EN: The mock council evaluator. JA: mock council evaluator です。</param>
    public EthosRejectScenario(CtgMockCouncilEvaluator? mockCouncilEvaluator = null)
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
            "ctg.scenario.ethos_reject",
            CouncilVoteValue.Approve,
            CouncilVoteValue.Reject,
            CouncilVoteValue.Approve);
}
