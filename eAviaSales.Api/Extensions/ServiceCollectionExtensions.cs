using eAviaSales.Api.Services.Cart;
using eAviaSales.Api.Services.Holds;
using eAviaSales.Api.Services.Orders;
using eAviaSales.Api.Services.Payments;
using eAviaSales.Api.Services.Refunds;
using eAviaSales.Api.Services.Ticketing;

namespace eAviaSales.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTicketingModuleScaffolding(this IServiceCollection services)
    {
        services.AddSingleton<TicketingMemoryStore>();
        services.AddSingleton<ICartService>(sp => sp.GetRequiredService<TicketingMemoryStore>());
        services.AddSingleton<IHoldService>(sp => sp.GetRequiredService<TicketingMemoryStore>());
        services.AddSingleton<IOrderService>(sp => sp.GetRequiredService<TicketingMemoryStore>());
        services.AddSingleton<IPaymentService>(sp => sp.GetRequiredService<TicketingMemoryStore>());
        services.AddSingleton<IRefundService>(sp => sp.GetRequiredService<TicketingMemoryStore>());
        return services;
    }
}
