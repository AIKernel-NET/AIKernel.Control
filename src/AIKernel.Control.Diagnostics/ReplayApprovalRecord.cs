namespace AIKernel.Control.Diagnostics;

/// <summary>[EN] Documents this public package API member. [JA] ReplayApprovalRecord を表します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Diagnostics.ReplayApprovalRecord']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Diagnostics.ReplayApprovalRecord']" />
public sealed record ReplayApprovalRecord(
    string ReplayLogHash,
    string ApprovedBy,
    string Decision);
