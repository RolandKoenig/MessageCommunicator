using Microsoft.Extensions.DependencyInjection;

namespace FirLib.Core.Services.SingleApplicationInstance;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMutexBasedSingleApplicationInstance(this IServiceCollection services,
        string mutexName)
    {
        services.AddSingleton<ISingleApplicationInstanceService, MutexBasedSingleApplicationInstanceService>(
            _ => new MutexBasedSingleApplicationInstanceService(mutexName));
        return services;
    }
}