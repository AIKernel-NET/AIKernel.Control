using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Emulator;

public sealed class ControlEmulatorEngine(
    INodeScheduler? scheduler = null,
    IControlPolicy? policy = null,
    IControlStateObserver? observer = null) : IControlEngine
{
    private readonly INodeScheduler _scheduler = scheduler ?? new DeterministicNodeScheduler();
    private readonly IControlPolicy _policy = policy ?? new AllowAllControlPolicy();

    public string EngineId => "control-emulator";

    public async ValueTask<ControlExecutionResult> ExecuteAsync(
        IExecutionGraph graph,
        ControlExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(request);

        var evaluation = await _policy
            .EvaluateAsync(graph, request, cancellationToken)
            .ConfigureAwait(false);

        if (!evaluation.Allowed)
        {
            return new ControlExecutionResult(
                request.ExecutionId,
                "Denied",
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
        }

        return new ControlExecutionResult(
            request.ExecutionId,
            "Completed",
            MergeMetadata(
                request.Metadata,
                new Dictionary<string, string>
                {
                    ["engine_id"] = EngineId,
                    ["graph_id"] = graph.GraphId,
                    ["scheduled_node_count"] = nodes.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)
                }));
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
