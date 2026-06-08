namespace AIKernel.Control.Diagnostics;

/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Diagnostics.ReplayApprovalRecord']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Diagnostics.ReplayApprovalRecord']" />
public sealed record ReplayApprovalRecord(
    string ReplayLogHash,
    string ApprovedBy,
    string Decision);
