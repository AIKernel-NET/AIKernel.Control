using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Core.Ctg;

internal static class CtgControlMetadata
{
    internal const string DefaultOperationId = "ctg.control.operation";
    internal const string DefaultStepId = "ctg.control.step";

    internal static IReadOnlyDictionary<string, string> Merge(
        params IReadOnlyDictionary<string, string>?[] sources)
    {
        var metadata = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var source in sources)
        {
            if (source is null)
            {
                continue;
            }

            foreach (var item in source)
            {
                metadata[item.Key] = item.Value;
            }
        }

        return metadata;
    }

    internal static string OperationId(CtgControlExecutionContext context)
        => FirstNonEmpty(
            context.OperationId,
            context.Request?.ExecutionId,
            DefaultOperationId);

    internal static string StepId(CtgControlExecutionContext context)
        => FirstNonEmpty(
            context.StepId,
            context.Graph?.GraphId,
            DefaultStepId);

    internal static DateTimeOffset ObservedAt(CtgControlExecutionContext context)
        => context.ObservedAt ?? DateTimeOffset.UtcNow;

    internal static CtgControlExecutionContext CreateContext(
        IExecutionGraph graph,
        ControlExecutionRequest request,
        CtgControlCoordinatorOptions options)
    {
        return new CtgControlExecutionContext
        {
            OperationId = FirstNonEmpty(request.ExecutionId, options.OperationId, DefaultOperationId),
            StepId = FirstNonEmpty(options.StepId, graph.GraphId, DefaultStepId),
            Graph = graph,
            Request = request,
            ProviderOutputs = options.ProviderOutputs,
            CanonReferences = options.CanonReferences,
            CorrelationId = options.CorrelationId,
            TraceId = options.TraceId,
            ObservedAt = options.ObservedAt,
            Metadata = Merge(options.Metadata, request.Metadata)
        };
    }

    internal static string FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return string.Empty;
    }
}
