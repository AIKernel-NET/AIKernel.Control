using AIKernel.Enums.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Creates deterministic CTG replay-oriented emulator scenarios.
/// [EN] Documents this public package API member. [JA] 決定論的な CTG replay 指向 emulator scenario を作成します。
/// </summary>
public sealed class CtgReplayScenario
{
    private readonly CtgMockCouncilEvaluator _mockCouncilEvaluator;

    /// <summary>
    /// EN: Initializes a replay scenario factory.
    /// [EN] Documents this public package API member. [JA] replay scenario factory を初期化します。
    /// </summary>
    /// <param name="mockCouncilEvaluator">EN: The mock council evaluator. JA: mock council evaluator です。</param>
    public CtgReplayScenario(CtgMockCouncilEvaluator? mockCouncilEvaluator = null)
    {
        _mockCouncilEvaluator = mockCouncilEvaluator ?? new CtgMockCouncilEvaluator();
    }

    /// <summary>
    /// EN: Creates a replay scenario with stable metadata.
    /// [EN] Documents this public package API member. [JA] 安定した metadata を持つ replay scenario を作成します。
    /// </summary>
    /// <returns>EN: The emulator scenario. JA: emulator scenario を返します。</returns>
    public CtgControlEmulatorScenario Create()
        => _mockCouncilEvaluator.CreateScenario(
            "ctg.scenario.replay",
            CouncilVoteValue.Approve,
            CouncilVoteValue.Approve,
            CouncilVoteValue.Abstain) with
        {
            Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["ctg.replay.seed"] = "ctg.scenario.replay"
            }
        };
}
