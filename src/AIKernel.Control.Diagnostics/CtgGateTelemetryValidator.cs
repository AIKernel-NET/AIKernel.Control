using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Validates that gate telemetry does not contain continuous provider carriers.
/// [EN] Documents this public package API member. [JA] gate telemetry に continuous provider carrier が含まれていないことを検証します。
/// </summary>
public sealed class CtgGateTelemetryValidator
{
    private static readonly string[] ContinuousCarrierMarkers =
    [
        "confidence",
        "risk",
        "risk_score",
        "score",
        "explanation"
    ];

    /// <summary>
    /// EN: Returns true when the decision gate result metadata contains no continuous provider carriers.
    /// [EN] Documents this public package API member. [JA] decision gate result metadata に continuous provider carrier が含まれない場合 true を返します。
    /// </summary>
    /// <param name="result">EN: The decision gate result. JA: decision gate result です。</param>
    /// <returns>EN: True when the metadata is gate-safe. JA: metadata が gate-safe の場合 true を返します。</returns>
    public bool IsGateTelemetryDiscreteOnly(DecisionGateResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.Metadata.Keys.All(IsDiscreteKey);
    }

    /// <summary>
    /// EN: Returns marker keys that violate the discrete-only gate telemetry rule.
    /// [EN] Documents this public package API member. [JA] discrete-only gate telemetry rule に違反する marker key を返します。
    /// </summary>
    /// <param name="result">EN: The decision gate result. JA: decision gate result です。</param>
    /// <returns>EN: The violating metadata keys. JA: 違反した metadata key を返します。</returns>
    public IReadOnlyList<string> FindContinuousCarrierKeys(DecisionGateResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return result.Metadata.Keys
            .Where(key => !IsDiscreteKey(key))
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsDiscreteKey(string key)
        => !ContinuousCarrierMarkers.Any(marker =>
            key.Contains(marker, StringComparison.OrdinalIgnoreCase));
}
