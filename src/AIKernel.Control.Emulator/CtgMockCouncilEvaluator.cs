using AIKernel.Control.Core.Ctg;
using AIKernel.Dtos.Control;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Creates deterministic mock council vote outputs for emulator scenarios.
/// EN: Documentation for public API. JA: emulator scenario 用の決定論的な mock council vote output を作成します。
/// </summary>
public sealed class CtgMockCouncilEvaluator
{
    /// <summary>
    /// EN: Creates provider vote outputs for the three CTG councils.
    /// EN: Documentation for public API. JA: CTG の 3 council 用 provider vote output を作成します。
    /// </summary>
    /// <param name="logos">EN: The Logos vote value. JA: Logos vote value です。</param>
    /// <param name="ethos">EN: The Ethos vote value. JA: Ethos vote value です。</param>
    /// <param name="pathos">EN: The Pathos vote value. JA: Pathos vote value です。</param>
    /// <returns>EN: Provider vote outputs. JA: provider vote output を返します。</returns>
    public IReadOnlyList<ProviderVoteOutput> CreateVotes(
        CouncilVoteValue logos,
        CouncilVoteValue ethos,
        CouncilVoteValue pathos)
    {
        return
        [
            CreateVote(CouncilKind.Logos, logos),
            CreateVote(CouncilKind.Ethos, ethos),
            CreateVote(CouncilKind.Pathos, pathos)
        ];
    }

    /// <summary>
    /// EN: Creates a provider vote output for one council.
    /// EN: Documentation for public API. JA: 1 つの council 用 provider vote output を作成します。
    /// </summary>
    /// <param name="councilKind">EN: The council kind. JA: council kind です。</param>
    /// <param name="voteValue">EN: The vote value. JA: vote value です。</param>
    /// <returns>EN: The provider vote output. JA: provider vote output を返します。</returns>
    public ProviderVoteOutput CreateVote(
        CouncilKind councilKind,
        CouncilVoteValue voteValue)
    {
        return new ProviderVoteOutput
        {
            ProviderId = $"mock.{councilKind.ToString().ToLowerInvariant()}",
            CouncilKind = councilKind,
            VoteValue = voteValue
        };
    }

    /// <summary>
    /// EN: Creates an emulator scenario from three vote values.
    /// EN: Documentation for public API. JA: 3 つの vote value から emulator scenario を作成します。
    /// </summary>
    /// <param name="scenarioId">EN: The scenario identifier. JA: scenario 識別子です。</param>
    /// <param name="logos">EN: The Logos vote value. JA: Logos vote value です。</param>
    /// <param name="ethos">EN: The Ethos vote value. JA: Ethos vote value です。</param>
    /// <param name="pathos">EN: The Pathos vote value. JA: Pathos vote value です。</param>
    /// <returns>EN: The emulator scenario. JA: emulator scenario を返します。</returns>
    public CtgControlEmulatorScenario CreateScenario(
        string scenarioId,
        CouncilVoteValue logos,
        CouncilVoteValue ethos,
        CouncilVoteValue pathos)
    {
        return new CtgControlEmulatorScenario
        {
            ScenarioId = scenarioId,
            ProviderOutputs = CreateVotes(logos, ethos, pathos),
            Request = new ControlExecutionRequest(
                scenarioId,
                new Dictionary<string, string>(StringComparer.Ordinal))
        };
    }
}
