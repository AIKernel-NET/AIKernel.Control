using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Formats CTG gate DTOs for diagnostics output.
/// JA: diagnostics output 用に CTG gate DTO を整形します。
/// </summary>
public sealed class CtgDiagnosticsFormatter
{
    private readonly CtgRejectReasonFormatter _reasonFormatter;

    /// <summary>
    /// EN: Initializes a CTG diagnostics formatter.
    /// JA: CTG diagnostics formatter を初期化します。
    /// </summary>
    /// <param name="reasonFormatter">EN: The reject reason formatter. JA: reject reason formatter です。</param>
    public CtgDiagnosticsFormatter(CtgRejectReasonFormatter? reasonFormatter = null)
    {
        _reasonFormatter = reasonFormatter ?? new CtgRejectReasonFormatter();
    }

    /// <summary>
    /// EN: Formats a decision gate result.
    /// JA: decision gate result を整形します。
    /// </summary>
    /// <param name="result">EN: The decision gate result. JA: decision gate result です。</param>
    /// <returns>EN: The formatted diagnostics line. JA: 整形された diagnostics line を返します。</returns>
    public string FormatDecisionGate(DecisionGateResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return $"operation={result.OperationId};decision={result.DecisionKind};accepted={result.Accepted};reason={FirstReason(result.RejectReasons)}";
    }

    /// <summary>
    /// EN: Formats a trajectory gate result.
    /// JA: trajectory gate result を整形します。
    /// </summary>
    /// <param name="result">EN: The trajectory gate result. JA: trajectory gate result です。</param>
    /// <returns>EN: The formatted diagnostics line. JA: 整形された diagnostics line を返します。</returns>
    public string FormatTrajectoryGate(TrajectoryGateResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return $"operation={result.OperationId};trajectory={result.DecisionKind};accepted={result.Accepted};reason={FirstReason(result.RejectReasons)}";
    }

    private string FirstReason(IReadOnlyList<RejectReasonInfo> reasons)
        => reasons.Count == 0 ? "none" : _reasonFormatter.Format(reasons[0]);
}
