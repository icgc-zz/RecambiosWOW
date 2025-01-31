// Infrastructure/Middleware/StartupMiddleware.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecambiosWOW.Core.Interfaces.Services;

namespace RecambiosWOW.Infrastructure.Middleware;

public class StartupMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IStartupService _startupService;
    private readonly ILogger<StartupMiddleware> _logger;
    private volatile bool _initialized;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public StartupMiddleware(
        RequestDelegate next,
        IStartupService startupService,
        ILogger<StartupMiddleware> logger)
    {
        _next = next;
        _startupService = startupService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_initialized)
        {
            try
            {
                await _semaphore.WaitAsync();
                if (!_initialized)
                {
                    await _startupService.InitializeAsync();
                    _initialized = true;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        await _next(context);
    }
}