using AIKernel.Dtos.Governance;
using System.Text;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Renders CTG governance traces into deterministic text.
/// EN: Documentation for public API. JA: CTG governance trace を決定論的な text に描画します。
/// </summary>
public sealed class CtgTraceRenderer
{
    private readonly CtgRejectReasonFormatter _reasonFormatter;

    /// <summary>
    /// EN: Initializes a CTG trace renderer.
    /// EN: Documentation for public API. JA: CTG trace renderer を初期化します。
    /// </summary>
    /// <param name="reasonFormatter">EN: The reject reason formatter. JA: reject reason formatter です。</param>
    public CtgTraceRenderer(CtgRejectReasonFormatter? reasonFormatter = null)
    {
        _reasonFormatter = reasonFormatter ?? new CtgRejectReasonFormatter();
    }

    /// <summary>
    /// EN: Renders a step governance trace.
    /// EN: Documentation for public API. JA: step governance trace を描画します。
    /// </summary>
    /// <param name="trace">EN: The step governance trace. JA: step governance trace です。</param>
    /// <returns>EN: The rendered trace. JA: 描画された trace を返します。</returns>
    public string RenderStep(StepGovernanceTrace trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var builder = new StringBuilder();
        builder.Append("step=").Append(trace.StepId)
            .Append(";decision=").Append(trace.DecisionGate.DecisionKind)
            .Append(";accepted=").Append(trace.DecisionGate.Accepted);

        foreach (var reason in trace.RejectReasons)
        {
            builder.Append(";reason=").Append(_reasonFormatter.Format(reason));
        }

        return builder.ToString();
    }

    /// <summary>
    /// EN: Renders a governance trace.
    /// EN: Documentation for public API. JA: governance trace を描画します。
    /// </summary>
    /// <param name="trace">EN: The governance trace. JA: governance trace です。</param>
    /// <returns>EN: The rendered trace. JA: 描画された trace を返します。</returns>
    public string RenderGovernanceTrace(GovernanceTrace trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var builder = new StringBuilder();
        builder.Append("trace=").Append(trace.TraceId)
            .Append(";decision=").Append(trace.DecisionKind)
            .Append(";accepted=").Append(trace.Accepted)
            .Append(";steps=").Append(trace.Steps.Count.ToString(System.Globalization.CultureInfo.InvariantCulture));

        foreach (var step in trace.Steps)
        {
            builder.AppendLine().Append(RenderStep(step));
        }

        return builder.ToString();
    }
}
