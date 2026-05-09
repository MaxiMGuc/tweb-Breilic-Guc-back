namespace eAviaSales.Api.Services.Cart;

public interface ICartService
{
    CartSnapshot Get(string cartId);
    CartSnapshot AddOrUpdateItem(
        string cartId,
        int eventId,
        string eventCode,
        int quantity,
        decimal unitPrice,
        string currencyCode);
    bool RemoveItem(string cartId, string itemId);

    void Clear(string cartId);
}
