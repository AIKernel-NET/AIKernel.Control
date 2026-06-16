using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Writes CTG governance DTOs into replay-safe metadata dictionaries.
/// [EN] Documents this public package API member. [JA] CTG governance DTO を replay-safe な metadata dictionary に書き出します。
/// </summary>
public sealed class CtgReplayMetadataWriter
{
    private readonly CtgRejectReasonFormatter _reasonFormatter;
    private readonly CtgCanonReferenceFormatter _canonReferenceFormatter;

    /// <summary>
    /// EN: Initializes a replay metadata writer.
    /// [EN] Documents this public package API member. [JA] replay metadata writer を初期化します。
    /// </summary>
    /// <param name="reasonFormatter">EN: The reject reason formatter. JA: reject reason formatter です。</param>
    /// <param name="canonReferenceFormatter">EN: The canon reference formatter. JA: canon reference formatter です。</param>
    public CtgReplayMetadataWriter(
        CtgRejectReasonFormatter? reasonFormatter = null,
        CtgCanonReferenceFormatter? canonReferenceFormatter = null)
    {
        _reasonFormatter = reasonFormatter ?? new CtgRejectReasonFormatter();
        _canonReferenceFormatter = canonReferenceFormatter ?? new CtgCanonReferenceFormatter();
    }

    /// <summary>
    /// EN: Writes a step governance trace to metadata.
    /// [EN] Documents this public package API member. [JA] step governance trace を metadata に書き出します。
    /// </summary>
    /// <param name="trace">EN: The step governance trace. JA: step governance trace です。</param>
    /// <returns>EN: Replay metadata. JA: replay metadata を返します。</returns>
    public IReadOnlyDictionary<string, string> WriteStep(StepGovernanceTrace trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var metadata = CreateBaseMetadata(trace.Metadata);
        metadata["ctg.trace_id"] = trace.TraceId;
        metadata["ctg.step_id"] = trace.StepId;
        metadata["ctg.decision.accepted"] = trace.DecisionGate.Accepted.ToString();
        metadata["ctg.decision.kind"] = trace.DecisionGate.DecisionKind.ToString();
        metadata["ctg.reject_reason.count"] = trace.RejectReasons.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        WriteRejectReasons(metadata, trace.RejectReasons);
        WriteCanonReferences(metadata, trace.CanonReferences);

        return metadata;
    }

    /// <summary>
    /// EN: Writes a trajectory gate result to metadata.
    /// [EN] Documents this public package API member. [JA] trajectory gate result を metadata に書き出します。
    /// </summary>
    /// <param name="result">EN: The trajectory gate result. JA: trajectory gate result です。</param>
    /// <returns>EN: Replay metadata. JA: replay metadata を返します。</returns>
    public IReadOnlyDictionary<string, string> WriteTrajectory(TrajectoryGateResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var metadata = CreateBaseMetadata(result.Metadata);
        metadata["ctg.operation_id"] = result.OperationId;
        metadata["ctg.trajectory.accepted"] = result.Accepted.ToString();
        metadata["ctg.trajectory.kind"] = result.DecisionKind.ToString();
        metadata["ctg.reject_reason.count"] = result.RejectReasons.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        WriteRejectReasons(metadata, result.RejectReasons);
        WriteCanonReferences(metadata, result.Trace.CanonReferences);

        return metadata;
    }

    private static Dictionary<string, string> CreateBaseMetadata(
        IReadOnlyDictionary<string, string>? source)
        => new(source ?? new Dictionary<string, string>(StringComparer.Ordinal), StringComparer.Ordinal);

    private void WriteRejectReasons(
        IDictionary<string, string> metadata,
        IReadOnlyList<RejectReasonInfo> rejectReasons)
    {
        for (var index = 0; index < rejectReasons.Count; index++)
        {
            var reason = rejectReasons[index];
            var prefix = $"ctg.reject_reason.{index}.";
            metadata[prefix + "kind"] = _reasonFormatter.FormatKind(reason.Kind);
            metadata[prefix + "code"] = reason.ReasonCode;
            metadata[prefix + "message"] = reason.Message;
        }
    }

    private void WriteCanonReferences(
        IDictionary<string, string> metadata,
        IReadOnlyList<CanonReference> references)
    {
        metadata["ctg.canon_reference.count"] =
            references.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);

        for (var index = 0; index < references.Count; index++)
        {
            metadata[$"ctg.canon_reference.{index}"] = _canonReferenceFormatter.Format(references[index]);
        }
    }
}
