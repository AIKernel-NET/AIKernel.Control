using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Emulator;

/// <summary>EN: Documentation for public API. JA: AllowAllControlPolicy を表します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.AllowAllControlPolicy']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Emulator.AllowAllControlPolicy']" />
public sealed class AllowAllControlPolicy : IControlPolicy
{
    /// <summary>EN: Documentation for public API. JA: EvaluateAsync を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Emulator.AllowAllControlPolicy.EvaluateAsync']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Emulator.AllowAllControlPolicy.EvaluateAsync']" />
    public ValueTask<ControlPolicyEvaluation> EvaluateAsync(
        IExecutionGraph graph,
        ControlExecutionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        return ValueTask.FromResult(new ControlPolicyEvaluation(
            true,
            "ALLOW",
            "ControlEmulator default policy."));
    }
}
