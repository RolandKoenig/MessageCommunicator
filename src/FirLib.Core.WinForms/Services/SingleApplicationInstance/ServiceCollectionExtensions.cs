using Microsoft.Extensions.DependencyInjection;

namespace FirLib.Core.Services.SingleApplicationInstance;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingleApplicationInstanceService_Using_WM_COPYDATA(
        this IServiceCollection services,
        string controlName)
    {
        services.AddSingleton<ISingleApplicationInstanceService, WmCopyDataSingleApplicationInstanceService>(
            _ => new WmCopyDataSingleApplicationInstanceService(controlName));
        return services;
    }
}
