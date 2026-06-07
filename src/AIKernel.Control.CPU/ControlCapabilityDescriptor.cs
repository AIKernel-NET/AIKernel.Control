namespace AIKernel.Control.CPU;

public sealed record ControlCapabilityDescriptor(
    string CapabilityId,
    string Operation,
    string InvocationMode);
