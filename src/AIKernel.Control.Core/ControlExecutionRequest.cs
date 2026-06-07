namespace AIKernel.Control;

public sealed record ControlExecutionRequest(
    string ExecutionId,
    IReadOnlyDictionary<string, string> Metadata);
