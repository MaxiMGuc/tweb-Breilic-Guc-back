using eAviaSales.Api.Services.Cart;

namespace eAviaSales.Api.Services.Orders;

public sealed class InMemoryOrderService : IOrderService
{
    public const string PendingPayment = "pending_payment";
    public const string Paid = "paid";
    public const string Cancelled = "cancelled";

    private readonly ICartService _cartService;
    private readonly object _sync = new();

    private readonly Dictionary<string, CheckoutRecord> _checkouts = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, OrderRecord> _orders = new(StringComparer.OrdinalIgnoreCase);

    public InMemoryOrderService(ICartService cartService)
    {
        _cartService = cartService;
    }

    public OrderServiceResult<CheckoutRecord> CreateCheckout(string customerKey)
    {
        var cart = _cartService.Get(customerKey);
        if (cart.Items.Count == 0)
        {
            return OrderServiceResult<CheckoutRecord>.Fail("cart_empty", "Cart has no items.");
        }

        var lines = cart.Items
            .Select(item => new OrderLineRecord
            {
                EventId = item.EventId,
                EventCode = item.EventCode,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            })
            .ToList();

        var currency = cart.Items[0].CurrencyCode;
        var total = cart.Items.Sum(static i => i.Quantity * i.UnitPrice);

        var checkout = new CheckoutRecord
        {
            CheckoutId = Guid.NewGuid().ToString("N"),
            CustomerKey = customerKey,
            Lines = lines,
            TotalAmount = total,
            CurrencyCode = currency,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15)
        };

        lock (_sync)
        {
            _checkouts[checkout.CheckoutId] = checkout;
        }

        return OrderServiceResult<CheckoutRecord>.Ok(checkout);
    }

    public OrderServiceResult<OrderRecord> PlaceOrder(string customerKey, string checkoutId)
    {
        CheckoutRecord checkout;
        lock (_sync)
        {
            if (!_checkouts.TryGetValue(checkoutId, out var c))
            {
                return OrderServiceResult<OrderRecord>.Fail("checkout_not_found", "Checkout not found.");
            }

            if (c.CustomerKey != customerKey)
            {
                return OrderServiceResult<OrderRecord>.Fail("checkout_forbidden", "Checkout does not belong to this session.");
            }

            if (c.ExpiresAtUtc <= DateTime.UtcNow)
            {
                _checkouts.Remove(checkoutId);
                return OrderServiceResult<OrderRecord>.Fail("checkout_expired", "Checkout has expired.");
            }

            checkout = c;
            _checkouts.Remove(checkoutId);
        }

        var order = new OrderRecord
        {
            OrderId = Guid.NewGuid().ToString("N"),
            CustomerKey = customerKey,
            Status = PendingPayment,
            Lines = checkout.Lines.Select(static l => new OrderLineRecord
            {
                EventId = l.EventId,
                EventCode = l.EventCode,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList(),
            TotalAmount = checkout.TotalAmount,
            CurrencyCode = checkout.CurrencyCode,
            CreatedAtUtc = DateTime.UtcNow
        };

        _cartService.Clear(customerKey);

        lock (_sync)
        {
            _orders[order.OrderId] = order;
        }

        return OrderServiceResult<OrderRecord>.Ok(order);
    }

    public OrderRecord? GetOrder(string orderId)
    {
        lock (_sync)
        {
            return _orders.TryGetValue(orderId, out var o) ? CloneOrder(o) : null;
        }
    }

    public IReadOnlyList<OrderRecord> ListOrdersForCustomer(string customerKey)
    {
        lock (_sync)
        {
            return _orders.Values
                .Where(o => o.CustomerKey == customerKey)
                .OrderByDescending(static o => o.CreatedAtUtc)
                .Select(CloneOrder)
                .ToList();
        }
    }

    public OrderServiceResult<object> CancelOrder(string customerKey, string orderId)
    {
        lock (_sync)
        {
            if (!_orders.TryGetValue(orderId, out var order))
            {
                return OrderServiceResult<object>.Fail("order_not_found", "Order was not found.");
            }

            if (order.CustomerKey != customerKey)
            {
                return OrderServiceResult<object>.Fail("order_forbidden", "Order does not belong to this session.");
            }

            if (order.Status != PendingPayment)
            {
                return OrderServiceResult<object>.Fail("invalid_order_state", $"Order cannot be cancelled in state {order.Status}.");
            }

            order.Status = Cancelled;
        }

        return OrderServiceResult<object>.Ok(null!);
    }

    public OrderServiceResult<IReadOnlyList<OrderTicketRecord>> IssueTickets(string orderId)
    {
        lock (_sync)
        {
            if (!_orders.TryGetValue(orderId, out var order))
            {
                return OrderServiceResult<IReadOnlyList<OrderTicketRecord>>.Fail("order_not_found", "Order was not found.");
            }

            if (order.Status != Paid)
            {
                return OrderServiceResult<IReadOnlyList<OrderTicketRecord>>.Fail(
                    "invalid_order_state",
                    "Tickets can only be issued for paid orders.");
            }

            if (order.Tickets.Count > 0)
            {
                return OrderServiceResult<IReadOnlyList<OrderTicketRecord>>.Ok(order.Tickets.ToList());
            }

            foreach (var line in order.Lines)
            {
                for (var i = 0; i < line.Quantity; i++)
                {
                    var ticketId = Guid.NewGuid().ToString("N");
                    var payload = $"{ticketId}:{line.EventId}:{order.OrderId}";
                    order.Tickets.Add(new OrderTicketRecord
                    {
                        TicketId = ticketId,
                        EventId = line.EventId,
                        EventCode = line.EventCode,
                        QrPayload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payload))
                    });
                }
            }

            return OrderServiceResult<IReadOnlyList<OrderTicketRecord>>.Ok(order.Tickets.ToList());
        }
    }

    public IReadOnlyList<OrderTicketRecord> GetTickets(string orderId)
    {
        lock (_sync)
        {
            if (!_orders.TryGetValue(orderId, out var order))
            {
                return [];
            }

            return order.Tickets.Select(static t => new OrderTicketRecord
            {
                TicketId = t.TicketId,
                EventId = t.EventId,
                EventCode = t.EventCode,
                QrPayload = t.QrPayload
            }).ToList();
        }
    }

    public bool MarkOrderPaid(string orderId)
    {
        lock (_sync)
        {
            if (!_orders.TryGetValue(orderId, out var order))
            {
                return false;
            }

            if (order.Status == Cancelled)
            {
                return false;
            }

            if (order.Status == Paid)
            {
                return true;
            }

            order.Status = Paid;
            return true;
        }
    }

    private static OrderRecord CloneOrder(OrderRecord source)
    {
        return new OrderRecord
        {
            OrderId = source.OrderId,
            CustomerKey = source.CustomerKey,
            Status = source.Status,
            Lines = source.Lines.Select(static l => new OrderLineRecord
            {
                EventId = l.EventId,
                EventCode = l.EventCode,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList(),
            TotalAmount = source.TotalAmount,
            CurrencyCode = source.CurrencyCode,
            CreatedAtUtc = source.CreatedAtUtc,
            Tickets = source.Tickets.Select(static t => new OrderTicketRecord
            {
                TicketId = t.TicketId,
                EventId = t.EventId,
                EventCode = t.EventCode,
                QrPayload = t.QrPayload
            }).ToList()
        };
    }
}
