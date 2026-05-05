var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "eAviaSales.Api is running");

app.Run();
