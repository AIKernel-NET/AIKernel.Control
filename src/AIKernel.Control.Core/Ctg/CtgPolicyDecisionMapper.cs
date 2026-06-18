using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Maps Core CTG gate results into the existing Control policy evaluation contract.
/// [EN] Documents this public package API member. [JA] Core CTG gate result を既存の Control policy evaluation contract に写像します。
/// </summary>
public sealed class CtgPolicyDecisionMapper
{
    /// <summary>
    /// EN: Maps a CTG control decision envelope to a Control policy evaluation.
    /// [EN] Documents this public package API member. [JA] CTG Control decision envelope を Control policy evaluation に写像します。
    /// </summary>
    /// <param name="envelope">EN: The CTG control decision envelope. JA: CTG Control decision envelope です。</param>
    /// <returns>EN: The mapped control policy evaluation. JA: 写像された control policy evaluation を返します。</returns>
    public ControlPolicyEvaluation Map(CtgControlDecisionEnvelope envelope)
    {
        ArgumentNullException.ThrowIfNull(envelope);

        return envelope.TrajectoryGate is { Accepted: false }
            ? new ControlPolicyEvaluation(false, "ABORT", CreateReason(envelope.TrajectoryGate.RejectReasons))
            : MapDecisionGate(envelope.DecisionGate);
    }

    /// <summary>
    /// EN: Maps a trajectory gate result to the Control halt policy code.
    /// [EN] Documents this public package API member. [JA] trajectory gate result を Control の halt policy code に写像します。
    /// </summary>
    /// <param name="trajectoryGate">EN: The Core trajectory gate result. JA: Core trajectory gate result です。</param>
    /// <returns>EN: The mapped control policy evaluation. JA: 写像された control policy evaluation を返します。</returns>
    public ControlPolicyEvaluation Map(TrajectoryGateResult trajectoryGate)
    {
        ArgumentNullException.ThrowIfNull(trajectoryGate);

        return trajectoryGate.Accepted
            ? new ControlPolicyEvaluation(true, "ALLOW", "CTG trajectory gate allowed execution.")
            : new ControlPolicyEvaluation(false, "ABORT", CreateReason(trajectoryGate.RejectReasons));
    }

    private static ControlPolicyEvaluation MapDecisionGate(DecisionGateResult decisionGate)
    {
        return decisionGate.Accepted
            ? new ControlPolicyEvaluation(true, "ALLOW", "CTG decision gate allowed execution.")
            : new ControlPolicyEvaluation(false, "DENY", CreateReason(decisionGate.RejectReasons));
    }

    private static string CreateReason(IReadOnlyList<RejectReasonInfo> rejectReasons)
        => rejectReasons.Count == 0
            ? "CTG gate did not accept execution."
            : CtgControlMetadata.FirstNonEmpty(
                rejectReasons[0].ReasonCode,
                rejectReasons[0].Message,
                rejectReasons[0].Kind.ToString());
}
