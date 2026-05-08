using eAviaSales.Api.Services.Holds;

namespace eAviaSales.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTicketingModuleScaffolding(this IServiceCollection services)
    {
        services.AddSingleton<IHoldService, InMemoryHoldService>();
        return services;
    }
}
