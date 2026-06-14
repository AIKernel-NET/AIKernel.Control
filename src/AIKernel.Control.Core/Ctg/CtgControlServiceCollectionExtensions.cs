using AIKernel.Abstractions.Control;
using AIKernel.Abstractions.Governance;
using AIKernel.Core.Governance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AIKernel.Control.Core.Ctg;

/// <summary>
/// EN: Provides dependency injection registration for opt-in CTG control governance.
/// JA: opt-in の CTG Control governance 用 DI 登録を提供します。
/// </summary>
public static class CtgControlServiceCollectionExtensions
{
    /// <summary>
    /// EN: Adds CTG control services without replacing existing registrations.
    /// JA: 既存登録を置き換えずに CTG Control service を追加します。
    /// </summary>
    /// <param name="services">EN: The service collection. JA: service collection です。</param>
    /// <returns>EN: The same service collection. JA: 同じ service collection を返します。</returns>
    public static IServiceCollection AddCtgControl(this IServiceCollection services)
        => AddCtgControl(services, new CtgControlCoordinatorOptions());

    /// <summary>
    /// EN: Adds CTG control services using explicit coordinator options.
    /// JA: 明示的な coordinator option を使用して CTG Control service を追加します。
    /// </summary>
    /// <param name="services">EN: The service collection. JA: service collection です。</param>
    /// <param name="options">EN: The coordinator options. JA: coordinator option です。</param>
    /// <returns>EN: The same service collection. JA: 同じ service collection を返します。</returns>
    public static IServiceCollection AddCtgControl(
        this IServiceCollection services,
        CtgControlCoordinatorOptions options)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(options);

        services.TryAddSingleton(options);
        services.TryAddSingleton<CtgCouncilEvaluationProviderRegistry>();
        services.TryAddSingleton<CtgCouncilEvaluationOrchestrator>();
        services.TryAddSingleton<ProviderVoteAdapter>();
        services.TryAddSingleton<CouncilDecisionBuilder>();
        services.TryAddSingleton<CouncilDecisionToGateInputAdapter>();
        services.TryAddSingleton<CtgStepTraceAssembler>();
        services.TryAddSingleton<IDecisionGate, CtgDecisionGateEvaluator>();
        services.TryAddSingleton<ITrajectoryGate, CtgTrajectoryGateEvaluator>();
        services.TryAddSingleton<CtgPolicyDecisionMapper>();
        services.TryAddSingleton<ICtgControlCoordinator, CtgControlCoordinator>();
        services.TryAddSingleton<CtgControlPolicyAdapter>();
        services.TryAddSingleton<CtgExecutionGatePolicy>();
        services.TryAddSingleton<IControlPolicy>(
            provider => provider.GetRequiredService<CtgControlPolicyAdapter>());

        return services;
    }
}
