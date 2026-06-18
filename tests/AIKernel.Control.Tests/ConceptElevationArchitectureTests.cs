namespace AIKernel.Control.Tests;

using System.Text.RegularExpressions;
using AIKernel.Control.Core.Concepts;

/// <summary>
/// EN: Verifies that Control concept names stay in orchestration-level concept surfaces.
/// JA: Control の概念名が orchestration-level concept surface に留まることを検証します。
/// </summary>
public sealed class ConceptElevationArchitectureTests
{
    private static readonly string[] PhilosophicalPrefixes =
    [
        "Ethos",
        "Pathos",
        "Logos",
        "Nomos",
        "Dike",
        "Kratos",
        "Aisthesis",
        "Phantasia",
        "Chronos",
        "Kairos",
        "Dynamis",
        "Energeia",
        "Nous",
        "Telos",
        "Apatheia",
        "Ataraxia",
        "Eidos",
    ];

    private static readonly string[] ForbiddenTechnicalSuffixes =
    [
        "Dto",
        "Request",
        "Result",
        "Mapper",
        "Adapter",
        "Serializer",
        "Converter",
        "HttpClient",
        "JSInterop",
        "JsInterop",
        "NativeBridge",
        "Provider",
    ];

    private static readonly ISet<string> CompatibilityExceptions = new HashSet<string>(StringComparer.Ordinal)
    {
        "EthosRejectScenario",
    };

    private static readonly Regex TypeDeclarationPattern = new(
        @"\b(?:public|internal|private|protected)?\s*(?:sealed\s+|abstract\s+|static\s+|partial\s+)*\b(?:class|record|interface|enum)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
        RegexOptions.Compiled);

    /// <summary>
    /// EN: Confirms Control concept facades do not duplicate Gate logic.
    /// JA: Control concept facade が Gate logic を複製しないことを確認します。
    /// </summary>
    [Fact]
    public void ConceptFacades_WhenUsed_ReturnDeterministicOrchestrationValues()
    {
        var trigger = new KairosTrigger();
        var scheduler = new KairosScheduler();
        var supervisor = new NousSupervisor();
        var strategy = new NousStrategy();
        var now = DateTimeOffset.Parse("2026-01-01T00:00:00Z", null, System.Globalization.DateTimeStyles.AssumeUniversal);

        Assert.True(trigger.CanFire(now, now.AddSeconds(-1)));
        Assert.Equal([now.AddSeconds(-1), now], scheduler.OrderCandidates([now, now.AddSeconds(-1)]));
        Assert.Equal("nous.supervisor.control", supervisor.Label("control"));
        Assert.Equal("alpha", strategy.SelectDeterministic(["zeta", "alpha"]));
    }

    /// <summary>
    /// EN: Rejects philosophical prefixes on low-level Control type names.
    /// JA: 低レイヤの Control 型名への哲学語 prefix を拒否します。
    /// </summary>
    [Fact]
    public void SourceTypes_WhenUsingPhilosophicalPrefix_DoNotUseForbiddenTechnicalSuffix()
    {
        var violations = FindViolations("AIKernel.Control.slnx");

        Assert.Empty(violations);
    }

    private static IReadOnlyList<string> FindViolations(string solutionFileName)
    {
        var repositoryRoot = FindRepositoryRoot(solutionFileName);
        var sourceRoot = Path.Combine(repositoryRoot, "src");

        return Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .SelectMany(path => FindViolationsInFile(repositoryRoot, path))
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private static IEnumerable<string> FindViolationsInFile(string repositoryRoot, string path)
    {
        var source = File.ReadAllText(path);
        foreach (Match match in TypeDeclarationPattern.Matches(source))
        {
            var typeName = match.Groups["name"].Value;
            if (CompatibilityExceptions.Contains(typeName))
            {
                continue;
            }

            var hasPhilosophicalPrefix = PhilosophicalPrefixes.Any(prefix => typeName.StartsWith(prefix, StringComparison.Ordinal));
            var hasForbiddenTechnicalSuffix = ForbiddenTechnicalSuffixes.Any(suffix => typeName.EndsWith(suffix, StringComparison.Ordinal));
            var isConceptSurface = path.Contains($"{Path.DirectorySeparatorChar}Concepts{Path.DirectorySeparatorChar}", StringComparison.Ordinal);

            if (hasPhilosophicalPrefix && (hasForbiddenTechnicalSuffix || !isConceptSurface))
            {
                yield return $"{Path.GetRelativePath(repositoryRoot, path)}: {typeName}";
            }
        }
    }

    private static string FindRepositoryRoot(string solutionFileName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, solutionFileName)))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"Could not locate {solutionFileName}.");
    }
}
