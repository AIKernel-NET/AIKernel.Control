using AIKernel.Enums.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Creates the discrete CTG truth-table scenario set for emulator validation.
/// EN: Documentation for public API. JA: emulator validation 用の離散 CTG truth-table scenario set を作成します。
/// </summary>
public sealed class CtgTruthTableScenario
{
    private static readonly CouncilVoteValue[] DiscreteVotes =
    [
        CouncilVoteValue.Approve,
        CouncilVoteValue.Abstain,
        CouncilVoteValue.Reject
    ];

    private readonly CtgMockCouncilEvaluator _mockCouncilEvaluator;

    /// <summary>
    /// EN: Initializes a truth table scenario factory.
    /// EN: Documentation for public API. JA: truth table scenario factory を初期化します。
    /// </summary>
    /// <param name="mockCouncilEvaluator">EN: The mock council evaluator. JA: mock council evaluator です。</param>
    public CtgTruthTableScenario(CtgMockCouncilEvaluator? mockCouncilEvaluator = null)
    {
        _mockCouncilEvaluator = mockCouncilEvaluator ?? new CtgMockCouncilEvaluator();
    }

    /// <summary>
    /// EN: Creates all 27 discrete vote combinations.
    /// EN: Documentation for public API. JA: 27 個の離散 vote combination をすべて作成します。
    /// </summary>
    /// <returns>EN: The scenario list. JA: scenario list を返します。</returns>
    public IReadOnlyList<CtgControlEmulatorScenario> CreateScenarios()
    {
        var scenarios = new List<CtgControlEmulatorScenario>();

        foreach (var logos in DiscreteVotes)
        {
            foreach (var ethos in DiscreteVotes)
            {
                foreach (var pathos in DiscreteVotes)
                {
                    scenarios.Add(_mockCouncilEvaluator.CreateScenario(
                        $"ctg.truth.{logos}.{ethos}.{pathos}",
                        logos,
                        ethos,
                        pathos));
                }
            }
        }

        return scenarios;
    }
}
