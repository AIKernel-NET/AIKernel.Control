namespace AIKernel.Control.Diagnostics;

public sealed record ReplayApprovalRecord(
    string ReplayLogHash,
    string ApprovedBy,
    string Decision);
