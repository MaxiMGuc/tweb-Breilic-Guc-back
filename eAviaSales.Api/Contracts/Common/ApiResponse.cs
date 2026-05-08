namespace eAviaSales.Api.Contracts.Common;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }
    public string? TraceId { get; init; }

    public static ApiResponse<T> Ok(T data, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            TraceId = traceId
        };
    }

    public static ApiResponse<T> Fail(ApiError error, string? traceId = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Error = error,
            TraceId = traceId
        };
    }
}
