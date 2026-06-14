using AIKernel.Abstractions.Control;
using AIKernel.Abstractions.Governance;
using AIKernel.Control.Core.Ctg;
using AIKernel.Core.Governance;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Runs CTG control scenarios through Core evaluators and the existing emulator engine.
/// JA: Core evaluator と既存 emulator engine を通じて CTG Control scenario を実行します。
/// </summary>
public sealed class CtgControlEmulator : ICtgControlEmulator
{
    private readonly ICtgControlCoordinator _coordinator;
    private readonly CtgPolicyDecisionMapper _policyDecisionMapper;
    private readonly ITrajectoryGate _trajectoryGate;

    /// <summary>
    /// EN: Initializes a CTG control emulator.
    /// JA: CTG Control emulator を初期化します。
    /// </summary>
    /// <param name="coordinator">EN: The CTG control coordinator. JA: CTG Control coordinator です。</param>
    /// <param name="policyDecisionMapper">EN: The policy decision mapper. JA: policy decision mapper です。</param>
    /// <param name="trajectoryGate">EN: The Core trajectory gate evaluator. JA: Core trajectory gate evaluator です。</param>
    public CtgControlEmulator(
        ICtgControlCoordinator? coordinator = null,
        CtgPolicyDecisionMapper? policyDecisionMapper = null,
        ITrajectoryGate? trajectoryGate = null)
    {
        _coordinator = coordinator ?? CreateDefaultCoordinator();
        _policyDecisionMapper = policyDecisionMapper ?? new CtgPolicyDecisionMapper();
        _trajectoryGate = trajectoryGate ?? new CtgTrajectoryGateEvaluator();
    }

    /// <summary>
    /// EN: Runs a CTG control scenario through policy, trajectory, and emulator surfaces.
    /// JA: policy、trajectory、emulator surface を通じて CTG Control scenario を実行します。
    /// </summary>
    /// <param name="scenario">EN: The scenario to run. JA: 実行する scenario です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The emulation result. JA: emulation result を返します。</returns>
    public async ValueTask<CtgEmulationResult> RunScenarioAsync(
        CtgControlEmulatorScenario scenario,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(scenario);
        cancellationToken.ThrowIfCancellationRequested();

        var context = CreateContext(scenario);
        var envelopeResult = await _coordinator
            .EvaluateAsync(context, cancellationToken)
            .ConfigureAwait(false);

        if (envelopeResult.IsFailure)
        {
            return CreateFaultedResult(scenario, envelopeResult.Error!.Message);
        }

        var envelope = envelopeResult.Value!;
        var trajectory = await EvaluateTrajectoryAsync(scenario, envelope, cancellationToken).ConfigureAwait(false);

        if (trajectory is { Accepted: false })
        {
            var evaluation = _policyDecisionMapper.Map(trajectory);
            return new CtgEmulationResult
            {
                ScenarioId = scenario.ScenarioId,
                DecisionEnvelope = envelope,
                TrajectoryGate = trajectory,
                ExecutionResult = CreatePolicySuppressedResult(scenario.Request, scenario.Graph, evaluation),
                ReplayMetadata = CreateReplayMetadata(scenario, envelope, trajectory)
            };
        }

        var observer = new RecordingObserver();
        var policy = CreatePolicy(scenario);
        var engine = new ControlEmulatorEngine(policy: policy, observer: observer);
        var executionResult = await engine
            .ExecuteAsync(scenario.Graph, scenario.Request, cancellationToken)
            .ConfigureAwait(false);

        return new CtgEmulationResult
        {
            ScenarioId = scenario.ScenarioId,
            ExecutionResult = executionResult,
            DecisionEnvelope = envelope,
            TrajectoryGate = trajectory,
            Snapshots = observer.Snapshots,
            ReplayMetadata = CreateReplayMetadata(scenario, envelope, trajectory)
        };
    }

    private static ICtgControlCoordinator CreateDefaultCoordinator()
    {
        return new CtgControlCoordinator(
            new ProviderVoteAdapter(),
            new CouncilDecisionBuilder(),
            new CouncilDecisionToGateInputAdapter(),
            new Core.Ctg.CtgStepTraceAssembler(),
            new CtgDecisionGateEvaluator());
    }

    private static CtgControlExecutionContext CreateContext(CtgControlEmulatorScenario scenario)
    {
        return new CtgControlExecutionContext
        {
            OperationId = scenario.Request.ExecutionId,
            StepId = scenario.Graph.GraphId,
            Graph = scenario.Graph,
            Request = scenario.Request,
            ProviderOutputs = scenario.ProviderOutputs,
            Metadata = scenario.Metadata
        };
    }

    private async ValueTask<TrajectoryGateResult?> EvaluateTrajectoryAsync(
        CtgControlEmulatorScenario scenario,
        CtgControlDecisionEnvelope envelope,
        CancellationToken cancellationToken)
    {
        if (scenario.TrajectorySteps is null)
        {
            return null;
        }

        var result = await _trajectoryGate
            .EvaluateAsync(
                new TrajectoryGateRequest
                {
                    OperationId = scenario.Request.ExecutionId,
                    Steps = scenario.TrajectorySteps,
                    TraceId = envelope.StepTrace.TraceId,
                    Metadata = scenario.Metadata
                },
                cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private CtgControlPolicyAdapter CreatePolicy(CtgControlEmulatorScenario scenario)
    {
        return new CtgControlPolicyAdapter(
            _coordinator,
            _policyDecisionMapper,
            new CtgControlCoordinatorOptions
            {
                OperationId = scenario.Request.ExecutionId,
                StepId = scenario.Graph.GraphId,
                ProviderOutputs = scenario.ProviderOutputs,
                Metadata = scenario.Metadata
            });
    }

    private static CtgEmulationResult CreateFaultedResult(
        CtgControlEmulatorScenario scenario,
        string message)
    {
        return new CtgEmulationResult
        {
            ScenarioId = scenario.ScenarioId,
            ExecutionResult = new ControlExecutionResult(
                scenario.Request.ExecutionId,
                "Faulted",
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["engine_id"] = "ctg-control-emulator",
                    ["graph_id"] = scenario.Graph.GraphId,
                    ["error_code"] = "CTG_EMULATION_FAULTED",
                    ["error_message"] = message
                })
        };
    }

    private static ControlExecutionResult CreatePolicySuppressedResult(
        ControlExecutionRequest request,
        IExecutionGraph graph,
        ControlPolicyEvaluation evaluation)
    {
        return new ControlExecutionResult(
            request.ExecutionId,
            string.Equals(evaluation.Code, "ABORT", StringComparison.OrdinalIgnoreCase)
                ? "Aborted"
                : "Denied",
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["engine_id"] = "ctg-control-emulator",
                ["graph_id"] = graph.GraphId,
                ["policy_code"] = evaluation.Code,
                ["policy_reason"] = evaluation.Reason
            });
    }

    private static IReadOnlyDictionary<string, string> CreateReplayMetadata(
        CtgControlEmulatorScenario scenario,
        CtgControlDecisionEnvelope envelope,
        TrajectoryGateResult? trajectory)
    {
        var metadata = new Dictionary<string, string>(scenario.Metadata, StringComparer.Ordinal)
        {
            ["ctg.scenario_id"] = scenario.ScenarioId,
            ["ctg.decision.accepted"] = envelope.DecisionGate.Accepted.ToString()
        };

        if (trajectory is not null)
        {
            metadata["ctg.trajectory.accepted"] = trajectory.Accepted.ToString();
            metadata["ctg.trajectory.kind"] = trajectory.DecisionKind.ToString();
        }

        return metadata;
    }

    private sealed class RecordingObserver : IControlStateObserver
    {
        private readonly List<ControlStateSnapshot> _snapshots = [];

        public IReadOnlyList<ControlStateSnapshot> Snapshots => _snapshots;

        public ValueTask ObserveAsync(
            ControlStateSnapshot snapshot,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _snapshots.Add(snapshot);
            return ValueTask.CompletedTask;
        }
    }
}
