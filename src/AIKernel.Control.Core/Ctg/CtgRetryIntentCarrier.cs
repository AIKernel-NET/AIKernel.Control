namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Carries high-priority retry intent as Control policy input without entering GateInput.
/// EN: Documentation for public API. JA: GateInput に入らない Control policy input として高優先 retry intent を保持します。
/// </summary>
public sealed record CtgRetryIntentCarrier
{
    /// <summary>
    /// EN: Gets whether retry is requested.
    /// EN: Documentation for public API. JA: retry が要求されているかどうかを取得します。
    /// </summary>
    public bool Requested { get; init; }

    /// <summary>
    /// EN: Gets the deterministic retry reason code.
    /// EN: Documentation for public API. JA: deterministic な retry reason code を取得します。
    /// </summary>
    public string ReasonCode { get; init; } = string.Empty;

    /// <summary>
    /// EN: Gets deterministic retry priority where larger values win.
    /// EN: Documentation for public API. JA: 値が大きいほど優先される deterministic retry priority を取得します。
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// EN: Gets normalized retry confidence outside CTG GateInput.
    /// EN: Documentation for public API. JA: CTG GateInput の外側に保持する正規化済み retry confidence を取得します。
    /// </summary>
    public double Confidence { get; init; }

    /// <summary>
    /// EN: Gets the source sensor name.
    /// EN: Documentation for public API. JA: source sensor 名を取得します。
    /// </summary>
    public string SourceSensor { get; init; } = string.Empty;

    /// <summary>
    /// EN: Gets deterministic retry metadata.
    /// EN: Documentation for public API. JA: deterministic retry metadata を取得します。
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
