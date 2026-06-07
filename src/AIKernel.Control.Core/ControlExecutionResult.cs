namespace AIKernel.Control;

public sealed record ControlExecutionResult(
    string ExecutionId,
    string Status,
    IReadOnlyDictionary<string, string> Metadata);
