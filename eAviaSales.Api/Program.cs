using eAviaSales.Data;
using eAviaSales.BusinessLogic.Functions.Auth;
using eAviaSales.BusinessLogic.Functions.Flights;
using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Api.Contracts.Common;
using eAviaSales.Api.Contracts.Errors;
using eAviaSales.Api.Extensions;
using eAviaSales.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var validationErrors = context.ModelState
            .Where(static pair => pair.Value?.Errors.Count > 0)
            .ToDictionary(
                static pair => pair.Key,
                static pair => pair.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        var response = ApiResponse<object>.Fail(new ApiError
        {
            Code = ApiErrorCodes.ValidationFailed,
            Message = "Validation failed",
            Details = validationErrors
        }, context.HttpContext.TraceIdentifier);

        return new BadRequestObjectResult(response);
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AviaSalesDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=eAviaSales.db");
});
builder.Services.AddScoped<IAuthActions, AuthFlow>();
builder.Services.AddScoped<IFlightActions, FlightFlow>();
builder.Services.AddTicketingModuleScaffolding();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
