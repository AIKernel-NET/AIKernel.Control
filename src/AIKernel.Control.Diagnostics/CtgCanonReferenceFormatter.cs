using AIKernel.Dtos.Governance;

namespace AIKernel.Control.Diagnostics;

/// <summary>
/// EN: Formats CTG canon references for diagnostics output.
/// EN: Documentation for public API. JA: diagnostics output 用に CTG CanonReference を整形します。
/// </summary>
public sealed class CtgCanonReferenceFormatter
{
    /// <summary>
    /// EN: Formats a canon reference as a stable compact string.
    /// EN: Documentation for public API. JA: CanonReference を安定した compact string として整形します。
    /// </summary>
    /// <param name="reference">EN: The canon reference. JA: CanonReference です。</param>
    /// <returns>EN: The formatted canon reference. JA: 整形された CanonReference を返します。</returns>
    public string Format(CanonReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        var path = string.IsNullOrWhiteSpace(reference.Path)
            ? reference.CanonId
            : reference.Path;
        var section = string.IsNullOrWhiteSpace(reference.Section)
            ? string.Empty
            : $"#{reference.Section}";
        var anchor = string.IsNullOrWhiteSpace(reference.Anchor)
            ? string.Empty
            : $"@{reference.Anchor}";

        return $"{path}{section}{anchor}";
    }
}
