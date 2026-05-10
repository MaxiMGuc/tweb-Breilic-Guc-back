using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Helpers;

/// <summary>Shared helper for stub endpoints (student project style).</summary>
public static class ControllerNotImplemented
{
    public static IActionResult Feature(string featureName) =>
        new ObjectResult(new ProblemDetails
        {
            Status = StatusCodes.Status501NotImplemented,
            Title = "Not implemented",
            Detail = $"{featureName} is not implemented yet."
        })
        {
            StatusCode = StatusCodes.Status501NotImplemented,
            ContentTypes = { "application/problem+json" }
        };
}
