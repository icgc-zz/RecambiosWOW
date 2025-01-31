using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using RecambiosWOW.Api.Extensions;
using RecambiosWOW.Application.Common.Markers;
using RecambiosWOW.Core.Interfaces;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Infrastructure.Common.Markers;
using RecambiosWOW.Infrastructure.Common.Settings;
using RecambiosWOW.Infrastructure.Providers.Database.Sqlite.Database.StartUp;
using RecambiosWOW.Infrastructure.Services.Database.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "RecambiosWow API v1",
        Version = "v1",
        Description = "API for RecambiosWow car parts management system"
    });

    // Enable annotations
    options.EnableAnnotations();
});

// Configure Swagger after service provider is built
builder.Services.ConfigureSwaggerGen(options =>
{
    // Additional Swagger configuration if needed
});

// Settings Configuration
// JWT Settings
builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .ValidateDataAnnotations();

// Database Settings
builder.Services
    .AddOptions<DatabaseSettings>()
    .Bind(builder.Configuration.GetSection("DatabaseSettings"))
    .ValidateDataAnnotations();

// Scrutor scanning
builder.Services
    .Scan(scan => scan
        // Application Layer Registration
        .FromAssemblyOf<IApplicationMarker>()
        .AddClasses(classes => classes
            .Where(type => !type.Name.EndsWith("Settings")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
        
        // Infrastructure Layer Registration
        .FromAssemblyOf<IInfrastructureMarker>()
        .AddClasses(classes => classes
            .Where(type => !type.Name.EndsWith("Settings")))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
    );

// Startup Services Registration (Singleton Lifetime)
builder.Services
    .Scan(scan => scan
        .FromAssemblyOf<IApplicationMarker>()
        .AddClasses(classes => classes
            .AssignableTo<IStartupService>())
        .AsImplementedInterfaces()
        .WithSingletonLifetime()

        .FromAssemblyOf<IInfrastructureMarker>()
        .AddClasses(classes => classes
            .AssignableTo<IStartupService>())
        .AsImplementedInterfaces()
        .WithSingletonLifetime()
    );

// Register SqliteStartupService as singleton since it's not picked up by Scrutor's scanning
builder.Services.AddSingleton<SqliteStartupService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // Configure SwaggerUI with API version support
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

// Add startup services early in the pipeline
app.UseStartupServices();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();