using eAviaSales.Data;
using eAviaSales.BusinessLogic.Functions.Auth;
using eAviaSales.BusinessLogic.Functions.Flights;
using eAviaSales.BusinessLogic.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AviaSalesDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=eAviaSales.db");
});
builder.Services.AddScoped<IAuthActions, AuthFlow>();
builder.Services.AddScoped<IFlightActions, FlightFlow>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
