namespace AIKernel.Control.Core.Perception;

using AIKernel.Control.Core.Ctg;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

/// <summary>
/// EN: Registers perception-to-CTG Control orchestration services.
/// [EN] Documents this public package API member. [JA] perception-to-CTG Control orchestration service を登録します。
/// </summary>
public static class PerceptionControlServiceCollectionExtensions
{
    /// <summary>
    /// EN: Adds perception Control adapters without registering perception execution providers.
    /// [EN] Documents this public package API member. [JA] perception execution Provider を登録せず perception Control adapter を追加します。
    /// </summary>
    /// <param name="services">EN: Service collection to update. JA: 更新対象の service collection です。</param>
    /// <returns>EN: The same service collection. JA: 同じ service collection を返します。</returns>
    public static IServiceCollection AddPerceptionCtgControl(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddCtgControl();
        services.TryAddSingleton<PerceptionControlAdapter>();
        services.TryAddSingleton<PerceptionCtgControlCoordinator>();
        services.TryAddSingleton<PerceptionPipelineSelector>();

        return services;
    }
}
