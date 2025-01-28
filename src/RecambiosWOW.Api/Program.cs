using RecambiosWOW.Application.Common.Markers;
using RecambiosWOW.Infrastructure.Common.Markers;
using RecambiosWOW.Infrastructure.Common.Settings;

// src/RecambiosWOW.Api/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services
    .AddControllers()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations();
// Scrutor scanning
builder.Services
    .Scan(scan => scan
        .FromAssemblyOf<IApplicationMarker>() // Interface in Application project
        .AddClasses(classes => classes.Where(type => !type.Name.EndsWith("Settings")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        
        .FromAssemblyOf<IInfrastructureMarker>() // Interface in Infrastructure project
        .AddClasses(classes => classes.Where(type => !type.Name.EndsWith("Settings")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    );

// Add specific configurations
builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();