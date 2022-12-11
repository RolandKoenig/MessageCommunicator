using Microsoft.Extensions.DependencyInjection;

namespace FirLib.Core.Services.ConfigurationFiles;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationConfigurationFileAccessors(
        this IServiceCollection services, string appName)
    {
        services.AddSingleton<IConfigurationFileAccessors, ConfigurationFileAccessors>(
            _ => new ConfigurationFileAccessors(appName));
        return services;
    }
}