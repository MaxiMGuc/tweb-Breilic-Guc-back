using eAviaSales.Api.Services.Holds;
using eAviaSales.Api.Services.Cart;
using eAviaSales.Api.Services.Orders;
using eAviaSales.Api.Services.Payments;
using eAviaSales.Api.Services.Refunds;

namespace eAviaSales.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTicketingModuleScaffolding(this IServiceCollection services)
    {
        services.AddSingleton<IHoldService, InMemoryHoldService>();
        services.AddSingleton<ICartService, InMemoryCartService>();
        services.AddSingleton<IOrderService, InMemoryOrderService>();
        services.AddSingleton<IPaymentService, InMemoryPaymentService>();
        services.AddSingleton<IRefundService, InMemoryRefundService>();
        return services;
    }
}
