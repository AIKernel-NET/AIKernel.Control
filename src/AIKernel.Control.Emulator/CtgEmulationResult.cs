using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;
using AIKernel.Control.Core.Ctg;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Carries the result of a CTG control emulator scenario.
/// EN: Documentation for public API. JA: CTG Control emulator scenario の結果を保持します。
/// </summary>
public sealed record CtgEmulationResult
{
    /// <summary>
    /// EN: Gets the scenario identifier.
    /// EN: Documentation for public API. JA: scenario 識別子を取得します。
    /// </summary>
    public string ScenarioId { get; init; } = string.Empty;

    /// <summary>
    /// EN: Gets the control execution result.
    /// EN: Documentation for public API. JA: control execution result を取得します。
    /// </summary>
    public ControlExecutionResult ExecutionResult { get; init; } = new(
        string.Empty,
        "Unknown",
        new Dictionary<string, string>(StringComparer.Ordinal));

    /// <summary>
    /// EN: Gets the CTG decision envelope produced before execution.
    /// EN: Documentation for public API. JA: execution 前に生成された CTG decision envelope を取得します。
    /// </summary>
    public CtgControlDecisionEnvelope? DecisionEnvelope { get; init; }

    /// <summary>
    /// EN: Gets the optional trajectory gate result.
    /// EN: Documentation for public API. JA: optional な trajectory gate result を取得します。
    /// </summary>
    public TrajectoryGateResult? TrajectoryGate { get; init; }

    /// <summary>
    /// EN: Gets snapshots emitted by the emulator execution path.
    /// EN: Documentation for public API. JA: emulator execution path が emit した snapshot を取得します。
    /// </summary>
    public IReadOnlyList<ControlStateSnapshot> Snapshots { get; init; } = [];

    /// <summary>
    /// EN: Gets replay metadata for the scenario result.
    /// EN: Documentation for public API. JA: scenario result の replay metadata を取得します。
    /// </summary>
    public IReadOnlyDictionary<string, string> ReplayMetadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
