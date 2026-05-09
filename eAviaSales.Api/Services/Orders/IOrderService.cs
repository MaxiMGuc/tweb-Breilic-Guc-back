namespace eAviaSales.Api.Services.Orders;

public interface IOrderService
{
    OrderServiceResult<CheckoutRecord> CreateCheckout(string customerKey);

    OrderServiceResult<OrderRecord> PlaceOrder(string customerKey, string checkoutId);

    OrderRecord? GetOrder(string orderId);

    IReadOnlyList<OrderRecord> ListOrdersForCustomer(string customerKey);

    OrderServiceResult<object> CancelOrder(string customerKey, string orderId);

    OrderServiceResult<IReadOnlyList<OrderTicketRecord>> IssueTickets(string orderId);

    IReadOnlyList<OrderTicketRecord> GetTickets(string orderId);

    /// <summary>Set order to Paid (called from payment confirmation / webhook).</summary>
    bool MarkOrderPaid(string orderId);
}
