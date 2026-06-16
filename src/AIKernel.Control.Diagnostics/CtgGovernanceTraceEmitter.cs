using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;
using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Emits CTG governance traces through the Control state observer surface.
/// [EN] Documents this public package API member. [JA] Control state observer surface を通じて CTG governance trace を emit します。
/// </summary>
public sealed class CtgGovernanceTraceEmitter
{
    private readonly CtgReplayMetadataWriter _metadataWriter;

    /// <summary>
    /// EN: Initializes a CTG governance trace emitter.
    /// [EN] Documents this public package API member. [JA] CTG governance trace emitter を初期化します。
    /// </summary>
    /// <param name="metadataWriter">EN: The replay metadata writer. JA: replay metadata writer です。</param>
    public CtgGovernanceTraceEmitter(CtgReplayMetadataWriter? metadataWriter = null)
    {
        _metadataWriter = metadataWriter ?? new CtgReplayMetadataWriter();
    }

    /// <summary>
    /// EN: Emits a step governance trace as a control state snapshot.
    /// [EN] Documents this public package API member. [JA] step governance trace を control state snapshot として emit します。
    /// </summary>
    /// <param name="observer">EN: The control state observer. JA: control state observer です。</param>
    /// <param name="executionId">EN: The execution identifier. JA: execution 識別子です。</param>
    /// <param name="graphId">EN: The graph identifier. JA: graph 識別子です。</param>
    /// <param name="trace">EN: The step governance trace. JA: step governance trace です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: A task that completes when the trace is emitted. JA: trace emit 完了時に完了する task を返します。</returns>
    public ValueTask EmitStepAsync(
        IControlStateObserver observer,
        string executionId,
        string graphId,
        StepGovernanceTrace trace,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(observer);
        ArgumentNullException.ThrowIfNull(trace);
        cancellationToken.ThrowIfCancellationRequested();

        return observer.ObserveAsync(
            new ControlStateSnapshot(
                executionId,
                graphId,
                trace.StepId,
                _metadataWriter.WriteStep(trace)),
            cancellationToken);
    }

    /// <summary>
    /// EN: Emits a trajectory gate result as a control state snapshot.
    /// [EN] Documents this public package API member. [JA] trajectory gate result を control state snapshot として emit します。
    /// </summary>
    /// <param name="observer">EN: The control state observer. JA: control state observer です。</param>
    /// <param name="executionId">EN: The execution identifier. JA: execution 識別子です。</param>
    /// <param name="graphId">EN: The graph identifier. JA: graph 識別子です。</param>
    /// <param name="result">EN: The trajectory gate result. JA: trajectory gate result です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: A task that completes when the trace is emitted. JA: trace emit 完了時に完了する task を返します。</returns>
    public ValueTask EmitTrajectoryAsync(
        IControlStateObserver observer,
        string executionId,
        string graphId,
        TrajectoryGateResult result,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(observer);
        ArgumentNullException.ThrowIfNull(result);
        cancellationToken.ThrowIfCancellationRequested();

        return observer.ObserveAsync(
            new ControlStateSnapshot(
                executionId,
                graphId,
                string.IsNullOrWhiteSpace(result.OperationId) ? "ctg.trajectory" : result.OperationId,
                _metadataWriter.WriteTrajectory(result)),
            cancellationToken);
    }
}
