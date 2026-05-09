using eAviaSales.Api.Services;

namespace eAviaSales.Api.Services.Orders;

public interface IOrderService
{
    ServiceResult<CheckoutRecord> CreateCheckout(string customerKey);

    ServiceResult<OrderRecord> PlaceOrder(string customerKey, string checkoutId);

    OrderRecord? GetOrder(string orderId);

    IReadOnlyList<OrderRecord> ListOrdersForCustomer(string customerKey);

    ServiceResult<object> CancelOrder(string customerKey, string orderId);

    ServiceResult<IReadOnlyList<OrderTicketRecord>> IssueTickets(string orderId);

    IReadOnlyList<OrderTicketRecord> GetTickets(string orderId);

    bool MarkOrderPaid(string orderId);
}
