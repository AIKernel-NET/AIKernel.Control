using AIKernel.Abstractions.Control;
using AIKernel.Control.Core.Ctg;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Describes a deterministic CTG control emulator scenario.
/// [EN] Documents this public package API member. [JA] 決定論的な CTG Control emulator scenario を記述します。
/// </summary>
public sealed record CtgControlEmulatorScenario
{
    /// <summary>
    /// EN: Gets the scenario identifier.
    /// [EN] Documents this public package API member. [JA] scenario 識別子を取得します。
    /// </summary>
    public string ScenarioId { get; init; } = "ctg.scenario";

    /// <summary>
    /// EN: Gets the execution graph used by the scenario.
    /// [EN] Documents this public package API member. [JA] scenario が使用する execution graph を取得します。
    /// </summary>
    public IExecutionGraph Graph { get; init; } = new EmulatedExecutionGraph(
        "ctg.graph",
        [new EmulatedExecutionNode("ctg.node", "ctg.operator", new Dictionary<string, string>(StringComparer.Ordinal))]);

    /// <summary>
    /// EN: Gets the control execution request used by the scenario.
    /// [EN] Documents this public package API member. [JA] scenario が使用する control execution request を取得します。
    /// </summary>
    public ControlExecutionRequest Request { get; init; } = new(
        "ctg.execution",
        new Dictionary<string, string>(StringComparer.Ordinal));

    /// <summary>
    /// EN: Gets provider vote outputs supplied to the CTG policy adapter.
    /// [EN] Documents this public package API member. [JA] CTG policy adapter に供給する provider vote output を取得します。
    /// </summary>
    public IReadOnlyList<ProviderVoteOutput> ProviderOutputs { get; init; } = [];

    /// <summary>
    /// EN: Gets optional step traces used for trajectory gate dry-run evaluation.
    /// [EN] Documents this public package API member. [JA] trajectory gate dry-run evaluation に使用する optional な step trace を取得します。
    /// </summary>
    public IReadOnlyList<StepGovernanceTrace>? TrajectorySteps { get; init; }

    /// <summary>
    /// EN: Gets scenario metadata.
    /// [EN] Documents this public package API member. [JA] scenario metadata を取得します。
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; init; } =
        new Dictionary<string, string>(StringComparer.Ordinal);
}
