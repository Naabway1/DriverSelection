using DriverSelection.Api.Configuration;
using Microsoft.Extensions.Options;

namespace DriverSelection.Api.Middleware;

public sealed class RequestLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly int _parallelLimit;

    private int _currentRequests;

    public RequestLimitMiddleware(
        RequestDelegate next,
        IOptions<Settings> settings)
    {
        _next = next;
        _parallelLimit = settings.Value.ParallelLimit;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var currentRequests = Interlocked.Increment(
            ref _currentRequests);

        if (currentRequests > _parallelLimit)
        {
            Interlocked.Decrement(ref _currentRequests);

            context.Response.StatusCode =
                StatusCodes.Status503ServiceUnavailable;

            await context.Response.WriteAsync(
                "Сервис временно недоступен");

            return;
        }

        try
        {
            await _next(context);
        }
        finally
        {
            Interlocked.Decrement(ref _currentRequests);
        }
    }
}