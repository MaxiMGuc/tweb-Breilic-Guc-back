using eAviaSales.Api.Contracts.Common;
using eAviaSales.Api.Contracts.Errors;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> OkResponse<T>(T payload)
    {
        return Ok(ApiResponse<T>.Ok(payload, HttpContext.TraceIdentifier));
    }

    protected IActionResult NotImplementedResponse(string featureName)
    {
        return StatusCode(
            StatusCodes.Status501NotImplemented,
            ApiResponse<object>.Fail(
                new ApiError
                {
                    Code = ApiErrorCodes.NotImplemented,
                    Message = $"{featureName} is not implemented yet."
                },
                HttpContext.TraceIdentifier));
    }
}
