namespace AIKernel.Control.Emulator;

public sealed record ControlPolicyDecision(
    bool Allowed,
    string Code,
    string Reason);

public static class ControlPolicy
{
    public static ControlPolicyDecision Allow(
        string reason)
        => new(true, "ALLOW", reason);

    public static ControlPolicyDecision Deny(
        string reason)
        => new(false, "DENY", reason);
}
