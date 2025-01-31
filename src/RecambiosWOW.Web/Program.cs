using RecambiosWOW.Shared.Services.Interfaces;
using RecambiosWOW.Web.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var services = app.Services;

services.AddScoped<IMetricsService, WebMetricsService>();

app.MapGet("/", () => "Hello World!");
app.Run();