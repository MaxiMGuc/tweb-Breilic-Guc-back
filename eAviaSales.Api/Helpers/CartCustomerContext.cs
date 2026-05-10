namespace eAviaSales.Api.Helpers;

public static class CartCustomerContext
{
    public const string HeaderName = "X-Cart-Id";

    /// <summary>Cart / anonymous customer bucket id (aligned with CartController).</summary>
    public static string Resolve(HttpContext httpContext)
    {
        var headerValue = httpContext.Request.Headers[HeaderName].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerValue))
        {
            return headerValue.Trim();
        }

        return $"guest:{httpContext.TraceIdentifier}";
    }
}
