namespace eAviaSales.Api.Contracts.Common;

public sealed class ApiError
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public IReadOnlyDictionary<string, string[]>? Details { get; init; }
}
