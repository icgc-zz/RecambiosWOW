// Api/Extensions/StartupServiceExtensions.cs

using RecambiosWOW.Core.Interfaces;
using RecambiosWOW.Infrastructure.Middleware;using RecambiosWOW.Application.Services.Startup;
using RecambiosWOW.Core.Interfaces.Services;

namespace RecambiosWOW.Api.Extensions;

public static class StartupServiceExtensions
{
    public static IServiceCollection AddStartupServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(StartupService))
            .AddClasses(classes => classes
                .AssignableTo<IStartupService>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
            
        return services;
    }

    public static IApplicationBuilder UseStartupServices(this IApplicationBuilder app)
    {
        return app.UseMiddleware<StartupMiddleware>();
    }
}