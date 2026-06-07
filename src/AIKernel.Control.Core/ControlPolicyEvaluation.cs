namespace AIKernel.Control;

public sealed record ControlPolicyEvaluation(
    bool Allowed,
    string Code,
    string Reason);
