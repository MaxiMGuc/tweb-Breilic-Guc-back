namespace eAviaSales.Api.Services.Orders;

public sealed class OrderServiceResult<T>
{
    public bool Success { get; init; }
    public T? Value { get; init; }
    public string? ErrorCode { get; init; }
    public string? Message { get; init; }

    public static OrderServiceResult<T> Ok(T value) =>
        new() { Success = true, Value = value };

    public static OrderServiceResult<T> Fail(string code, string message) =>
        new() { Success = false, ErrorCode = code, Message = message };
}
