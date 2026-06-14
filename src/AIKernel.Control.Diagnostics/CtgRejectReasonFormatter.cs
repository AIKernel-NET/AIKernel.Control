using AIKernel.Dtos.Governance;
using AIKernel.Enums.Governance;
using System.Text;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Formats CTG reject reasons for replay and diagnostics views.
/// JA: replay と diagnostics view 用に CTG reject reason を整形します。
/// </summary>
public sealed class CtgRejectReasonFormatter
{
    /// <summary>
    /// EN: Formats a reject reason as a compact diagnostic string.
    /// JA: reject reason を簡潔な diagnostic string として整形します。
    /// </summary>
    /// <param name="reason">EN: The reject reason. JA: reject reason です。</param>
    /// <returns>EN: The formatted reject reason. JA: 整形された reject reason を返します。</returns>
    public string Format(RejectReasonInfo reason)
    {
        ArgumentNullException.ThrowIfNull(reason);

        var code = string.IsNullOrWhiteSpace(reason.ReasonCode)
            ? FormatKind(reason.Kind)
            : reason.ReasonCode;
        var message = string.IsNullOrWhiteSpace(reason.Message)
            ? reason.Kind.ToString()
            : reason.Message;

        return $"{FormatKind(reason.Kind)}:{code}:{message}";
    }

    /// <summary>
    /// EN: Formats a reject reason kind using uppercase snake case metadata form.
    /// JA: reject reason kind を metadata 用の uppercase snake case で整形します。
    /// </summary>
    /// <param name="kind">EN: The reject reason kind. JA: reject reason kind です。</param>
    /// <returns>EN: The uppercase snake case name. JA: uppercase snake case 名を返します。</returns>
    public string FormatKind(RejectReasonKind kind)
        => ToUpperSnakeCase(kind.ToString());

    private static string ToUpperSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "UNKNOWN";
        }

        var builder = new StringBuilder(value.Length + 8);

        for (var index = 0; index < value.Length; index++)
        {
            var current = value[index];

            if (char.IsUpper(current) && index > 0)
            {
                builder.Append('_');
            }

            builder.Append(char.ToUpperInvariant(current));
        }

        return builder.ToString();
    }
}
