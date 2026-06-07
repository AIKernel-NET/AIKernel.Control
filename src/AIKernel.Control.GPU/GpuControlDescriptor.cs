namespace AIKernel.Control.GPU;

public sealed record GpuControlDescriptor(
    string DeviceId,
    string ExecutionMode,
    IReadOnlyDictionary<string, string> Metadata);
