namespace AIKernel.Control.Core.Concepts;

/// <summary>
/// [Timing layer - Kairos / カイロス]
/// [EN] Concept facade for optimal trigger timing in Control orchestration.
/// [JA] Control orchestration における最適 trigger timing の概念 facade です。
/// Old technical name: Trigger.
/// Do not use this term for DTO, Mapper, Adapter, Gate decision, or Provider implementation names.
/// </summary>
public sealed class KairosTrigger
{
    /// <summary>
    /// [EN] Returns whether the trigger can fire at the supplied time.
    /// [JA] 指定時刻で trigger を発火できるかを返します。
    /// </summary>
    public bool CanFire(DateTimeOffset now, DateTimeOffset earliestAllowedTime)
        => now >= earliestAllowedTime;
}

/// <summary>
/// [Timing layer - Kairos / カイロス]
/// [EN] Concept facade for deterministic scheduling windows, not Gate logic.
/// [JA] Gate logic ではなく deterministic scheduling window を扱う概念 facade です。
/// Old technical name: Scheduler.
/// Do not use this term for DTO, Mapper, Adapter, HttpClient, or Gate decision names.
/// </summary>
public sealed class KairosScheduler
{
    /// <summary>
    /// [EN] Orders timing candidates deterministically.
    /// [JA] timing candidate を決定論的に整列します。
    /// </summary>
    public IReadOnlyList<DateTimeOffset> OrderCandidates(IEnumerable<DateTimeOffset> candidates)
        => candidates.Order().ToArray();
}

/// <summary>
/// [Supervision layer - Nous / ヌース]
/// [EN] Concept facade for supervision responsibility in Control orchestration.
/// [JA] Control orchestration における supervision の責務を表す概念 facade です。
/// Old technical name: Supervisor.
/// Do not use this term for DTO, Mapper, Adapter, Gate decision, or Provider implementation names.
/// </summary>
public sealed class NousSupervisor
{
    /// <summary>
    /// [EN] Returns a stable supervision label for diagnostics and documentation.
    /// [JA] diagnostics / documentation 用の安定した supervision label を返します。
    /// </summary>
    public string Label(string scope)
        => string.IsNullOrWhiteSpace(scope)
            ? "nous.supervisor"
            : $"nous.supervisor.{scope}";
}

/// <summary>
/// [Supervision layer - Nous / ヌース]
/// [EN] Concept facade for high-level strategy selection without duplicating Gate logic.
/// [JA] Gate logic を複製せず high-level strategy selection を表す概念 facade です。
/// Old technical name: Strategy.
/// Do not use this term for DTO, Mapper, Adapter, Gate decision, or Provider implementation names.
/// </summary>
public sealed class NousStrategy
{
    /// <summary>
    /// [EN] Selects the first deterministic strategy name from an already validated list.
    /// [JA] 検証済み list から最初の deterministic strategy name を選択します。
    /// </summary>
    public string SelectDeterministic(IEnumerable<string> strategyNames)
        => strategyNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Order(StringComparer.Ordinal)
            .FirstOrDefault() ?? string.Empty;
}
