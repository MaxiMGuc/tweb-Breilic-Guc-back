namespace eAviaSales.Api.Services;

/// <summary>Simple success/fail wrapper for in-memory ticketing flows (student-project style).</summary>
public readonly record struct ServiceResult<T>(bool Success, T? Value, string? ErrorCode, string? Message)
{
    public static ServiceResult<T> Ok(T value) => new(true, value, null, null);

    public static ServiceResult<T> Fail(string code, string msg) => new(false, default, code, msg);
}
