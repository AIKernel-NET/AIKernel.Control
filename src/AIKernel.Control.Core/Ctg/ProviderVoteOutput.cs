using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Carries a provider-supplied council vote candidate before normalization.
/// JA: 正規化前の provider 由来 council vote 候補を保持します。
/// </summary>
public sealed record ProviderVoteOutput
{
    /// <summary>
    /// EN: Gets the provider identifier that supplied the vote.
    /// JA: vote を供給した provider 識別子を取得します。
    /// </summary>
    public string ProviderId { get; init; } = string.Empty;

    /// <summary>
    /// EN: Gets the council represented by the provider output.
    /// JA: provider output が表す council を取得します。
    /// </summary>
    public CouncilKind CouncilKind { get; init; } = CouncilKind.Unknown;

    /// <summary>
    /// EN: Gets the provider-supplied vote value.
    /// JA: provider が供給した vote value を取得します。
    /// </summary>
    public CouncilVoteValue VoteValue { get; init; } = CouncilVoteValue.Unknown;

    /// <summary>
    /// EN: Gets canonical references supplied with the provider vote.
    /// JA: provider vote とともに供給された CanonReference を取得します。
    /// </summary>
    public IReadOnlyList<CanonReference> CanonReferences { get; init; } = [];

    /// <summary>
    /// EN: Gets discrete provider metadata kept outside the GateInput carrier.
    /// JA: GateInput carrier の外側に保持する discrete provider metadata を取得します。
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
