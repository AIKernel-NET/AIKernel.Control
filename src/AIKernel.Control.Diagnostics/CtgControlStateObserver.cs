using AIKernel.Abstractions.Control;
using AIKernel.Dtos.Control;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Captures control state snapshots for CTG diagnostics and optional forwarding.
/// EN: Documentation for public API. JA: CTG diagnostics 用に control state snapshot を捕捉し、必要に応じて転送します。
/// </summary>
public sealed class CtgControlStateObserver : IControlStateObserver
{
    private readonly IControlStateObserver? _innerObserver;
    private readonly List<ControlStateSnapshot> _snapshots = [];

    /// <summary>
    /// EN: Initializes a CTG control state observer.
    /// EN: Documentation for public API. JA: CTG control state observer を初期化します。
    /// </summary>
    /// <param name="innerObserver">EN: The optional downstream observer. JA: optional な downstream observer です。</param>
    public CtgControlStateObserver(IControlStateObserver? innerObserver = null)
    {
        _innerObserver = innerObserver;
    }

    /// <summary>
    /// EN: Gets captured snapshots.
    /// EN: Documentation for public API. JA: 捕捉された snapshot を取得します。
    /// </summary>
    public IReadOnlyList<ControlStateSnapshot> Snapshots => _snapshots;

    /// <summary>
    /// EN: Captures and optionally forwards a control state snapshot.
    /// EN: Documentation for public API. JA: control state snapshot を捕捉し、必要に応じて転送します。
    /// </summary>
    /// <param name="snapshot">EN: The control state snapshot. JA: control state snapshot です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: A task that completes when observation finishes. JA: observation 完了時に完了する task を返します。</returns>
    public async ValueTask ObserveAsync(
        ControlStateSnapshot snapshot,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        cancellationToken.ThrowIfCancellationRequested();

        _snapshots.Add(snapshot);

        if (_innerObserver is not null)
        {
            await _innerObserver.ObserveAsync(snapshot, cancellationToken).ConfigureAwait(false);
        }
    }
}
