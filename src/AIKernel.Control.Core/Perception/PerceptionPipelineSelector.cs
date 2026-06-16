namespace AIKernel.Control.Core.Perception;

using AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Selects a pipeline carrier from Core CTG output without duplicating Gate rules.
/// [EN] Documents this public package API member. [JA] Gate rule を複製せず Core CTG output から pipeline carrier を選択します。
/// </summary>
public sealed class PerceptionPipelineSelector
{
    /// <summary>
    /// EN: Selects a dynamic pipeline mode by reading the existing Core decision result.
    /// [EN] Documents this public package API member. [JA] 既存の Core decision result を読み取り dynamic pipeline mode を選択します。
    /// </summary>
    /// <param name="envelope">EN: CTG control decision envelope. JA: CTG Control decision envelope です。</param>
    /// <returns>EN: Perception pipeline selection. JA: perception pipeline selection を返します。</returns>
    public PerceptionPipelineSelection Select(CtgControlDecisionEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        var retryRequested = envelope.RetryIntent?.Requested == true;
        var mode = retryRequested
            ? "retry"
            : envelope.DecisionGate.Accepted
            ? "execute"
            : "halt";

        return new PerceptionPipelineSelection
        {
            Mode = mode,
            DecisionEnvelope = envelope,
            RetryIntent = envelope.RetryIntent,
            Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["decision"] = envelope.DecisionGate.DecisionKind.ToString(),
                ["retryRequested"] = retryRequested.ToString(),
                ["retryReasonCode"] = envelope.RetryIntent?.ReasonCode ?? string.Empty,
                ["source"] = retryRequested ? "sensor-retry-intent" : "core-decision-gate"
            }
        };
    }
}
