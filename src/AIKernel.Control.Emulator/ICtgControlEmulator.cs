namespace AIKernel.Control.Emulator;

/// <summary>
/// EN: Runs deterministic CTG control emulator scenarios.
/// [EN] Documents this public package API member. [JA] 決定論的な CTG Control emulator scenario を実行します。
/// </summary>
public interface ICtgControlEmulator
{
    /// <summary>
    /// EN: Runs a CTG control emulator scenario.
    /// [EN] Documents this public package API member. [JA] CTG Control emulator scenario を実行します。
    /// </summary>
    /// <param name="scenario">EN: The scenario to run. JA: 実行する scenario です。</param>
    /// <param name="cancellationToken">EN: The cancellation token. JA: キャンセル通知を監視するトークンです。</param>
    /// <returns>EN: The emulation result. JA: emulation result を返します。</returns>
    ValueTask<CtgEmulationResult> RunScenarioAsync(
        CtgControlEmulatorScenario scenario,
        CancellationToken cancellationToken);
}
