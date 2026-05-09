namespace eAviaSales.Api.Services.Cart;

public sealed class InMemoryCartService : ICartService
{
    private readonly object _sync = new();
    private readonly Dictionary<string, Dictionary<string, CartItem>> _storage = new(StringComparer.OrdinalIgnoreCase);

    public CartSnapshot Get(string cartId)
    {
        lock (_sync)
        {
            if (!_storage.TryGetValue(cartId, out var cart))
            {
                return new CartSnapshot
                {
                    CartId = cartId
                };
            }

            return new CartSnapshot
            {
                CartId = cartId,
                Items = cart.Values.Select(Clone).ToList()
            };
        }
    }

    public CartSnapshot AddOrUpdateItem(
        string cartId,
        int eventId,
        string eventCode,
        int quantity,
        decimal unitPrice,
        string currencyCode)
    {
        lock (_sync)
        {
            if (!_storage.TryGetValue(cartId, out var cart))
            {
                cart = new Dictionary<string, CartItem>(StringComparer.OrdinalIgnoreCase);
                _storage[cartId] = cart;
            }

            var existing = cart.Values.FirstOrDefault(item => item.EventId == eventId);
            if (existing is not null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                var item = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString("N"),
                    EventId = eventId,
                    EventCode = eventCode,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    CurrencyCode = currencyCode
                };

                cart[item.ItemId] = item;
            }

            return new CartSnapshot
            {
                CartId = cartId,
                Items = cart.Values.Select(Clone).ToList()
            };
        }
    }

    public bool RemoveItem(string cartId, string itemId)
    {
        lock (_sync)
        {
            return _storage.TryGetValue(cartId, out var cart) && cart.Remove(itemId);
        }
    }

    public void Clear(string cartId)
    {
        lock (_sync)
        {
            _storage.Remove(cartId);
        }
    }

    private static CartItem Clone(CartItem source)
    {
        return new CartItem
        {
            ItemId = source.ItemId,
            EventId = source.EventId,
            EventCode = source.EventCode,
            Quantity = source.Quantity,
            UnitPrice = source.UnitPrice,
            CurrencyCode = source.CurrencyCode
        };
    }
}
