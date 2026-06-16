using AIKernel.Dtos.Control;
using System.Text;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Formats Control state snapshots as a deterministic CTG timeline.
/// [EN] Documents this public package API member. [JA] Control state snapshot を決定論的な CTG timeline として整形します。
/// </summary>
public sealed class CtgControlTimelineFormatter
{
    /// <summary>
    /// EN: Formats snapshots in their observed order.
    /// [EN] Documents this public package API member. [JA] snapshot を観測順で整形します。
    /// </summary>
    /// <param name="snapshots">EN: The control state snapshots. JA: control state snapshot です。</param>
    /// <returns>EN: The formatted timeline. JA: 整形された timeline を返します。</returns>
    public string Format(IReadOnlyList<ControlStateSnapshot> snapshots)
    {
        ArgumentNullException.ThrowIfNull(snapshots);

        var builder = new StringBuilder();

        foreach (var snapshot in snapshots)
        {
            builder.Append(snapshot.ExecutionId)
                .Append(':')
                .Append(snapshot.GraphId)
                .Append(':')
                .Append(snapshot.NodeId)
                .AppendLine();
        }

        return builder.ToString();
    }
}
