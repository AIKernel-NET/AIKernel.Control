using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Emulator;

/// <summary>EN: Documentation for public API. JA: ControlEmulatorEngine を表します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.ControlEmulatorEngine']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.ControlEmulatorEngine']" />
public sealed class ControlEmulatorEngine(
    INodeScheduler? scheduler = null,
    IControlPolicy? policy = null,
    IControlStateObserver? observer = null) : IControlEngine
{
    private readonly INodeScheduler _scheduler = scheduler ?? new DeterministicNodeScheduler();
    private readonly IControlPolicy _policy = policy ?? new AllowAllControlPolicy();

    /// <summary>EN: Documentation for public API. JA: EngineId を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Emulator.ControlEmulatorEngine.EngineId']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Emulator.ControlEmulatorEngine.EngineId']" />
    public string EngineId => "control-emulator";

    /// <summary>EN: Documentation for public API. JA: ExecuteAsync を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Emulator.ControlEmulatorEngine.ExecuteAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Emulator.ControlEmulatorEngine.ExecuteAsync']" />
    public async ValueTask<ControlExecutionResult> ExecuteAsync(
        IExecutionGraph graph,
        ControlExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var evaluation = await _policy
                .EvaluateAsync(graph, request, cancellationToken)
                .ConfigureAwait(false);

            if (!evaluation.Allowed)
            {
                return new ControlExecutionResult(
                    request.ExecutionId,
                    PolicyStatus(evaluation),
                    MergeMetadata(
                        request.Metadata,
                        new Dictionary<string, string>
                        {
                            ["engine_id"] = EngineId,
                            ["graph_id"] = graph.GraphId,
                            ["policy_code"] = evaluation.Code,
                            ["policy_reason"] = evaluation.Reason
                        }));
            }

            var nodes = await _scheduler
                .ScheduleAsync(graph, cancellationToken)
                .ConfigureAwait(false);

            var executionMetadata = new Dictionary<string, string>
            {
                ["engine_id"] = EngineId,
                ["graph_id"] = graph.GraphId,
                ["scheduled_node_count"] = nodes.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)
            };

            foreach (var node in nodes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (observer is not null)
                {
                    await observer
                        .ObserveAsync(
                            new ControlStateSnapshot(
                                request.ExecutionId,
                                graph.GraphId,
                                node.NodeId,
                                node.Metadata),
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                RecordNodeMetadata(executionMetadata, node);
                ExecuteEmulatedNode(node);
            }

            return new ControlExecutionResult(
                request.ExecutionId,
                "Completed",
                MergeMetadata(request.Metadata, executionMetadata));
        }
        catch (OperationCanceledException)
        {
            return new ControlExecutionResult(
                request.ExecutionId,
                "Canceled",
                MergeMetadata(
                    request.Metadata,
                    new Dictionary<string, string>
                    {
                        ["engine_id"] = EngineId,
                        ["graph_id"] = graph.GraphId,
                        ["error_code"] = "CONTROL_CANCELED",
                        ["error_message"] = "Control execution was canceled."
                    }));
        }
        catch (Exception ex)
        {
            return new ControlExecutionResult(
                request.ExecutionId,
                "Faulted",
                MergeMetadata(
                    request.Metadata,
                    new Dictionary<string, string>
                    {
                        ["engine_id"] = EngineId,
                        ["graph_id"] = graph.GraphId,
                        ["error_code"] = "CONTROL_FAULTED",
                        ["error_message"] = ex.Message
                    }));
        }
    }

    private static string PolicyStatus(ControlPolicyEvaluation evaluation)
    {
        return string.Equals(evaluation.Code, "ABORT", StringComparison.OrdinalIgnoreCase)
            ? "Aborted"
            : "Denied";
    }

    private static void ExecuteEmulatedNode(IExecutionNode node)
    {
        if (node is EmulatedExecutionNode emulated &&
            !string.IsNullOrEmpty(emulated.FaultMessage))
        {
            throw new InvalidOperationException(emulated.FaultMessage);
        }
    }

    private static void RecordNodeMetadata(
        IDictionary<string, string> metadata,
        IExecutionNode node)
    {
        metadata["last_node_id"] = node.NodeId;
        metadata["last_operator_id"] = node.OperatorId;

        var prefix = $"node.{node.NodeId}.";
        foreach (var item in node.Metadata)
        {
            metadata[prefix + item.Key] = item.Value;
        }
    }

    private static IReadOnlyDictionary<string, string> MergeMetadata(
        IReadOnlyDictionary<string, string>? first,
        IReadOnlyDictionary<string, string> second)
    {
        var metadata = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var item in first ?? new Dictionary<string, string>())
        {
            metadata[item.Key] = item.Value;
        }

        foreach (var item in second)
        {
            metadata[item.Key] = item.Value;
        }

        return metadata;
    }
}
