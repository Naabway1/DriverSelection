using DriverSelection.Api.Configuration;
using DriverSelection.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace DriverSelection.Tests.Middleware;

[TestFixture]
public class RequestLimitMiddlewareTests
{
    [Test]
    public async Task Should_Return503_When_ParallelLimitExceeded()
    {
        var firstTwoRequestsStarted = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var releaseRequests = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        var startedRequests = 0;

        RequestDelegate next = async _ =>
        {
            if (Interlocked.Increment(ref startedRequests) == 2)
            {
                firstTwoRequestsStarted.TrySetResult(true);
            }

            await releaseRequests.Task;
        };

        var settings = Options.Create(new Settings
        {
            ParallelLimit = 2
        });

        var middleware = new RequestLimitMiddleware(next, settings);

        var context1 = new DefaultHttpContext();
        var context2 = new DefaultHttpContext();
        var context3 = new DefaultHttpContext();

        var request1 = middleware.InvokeAsync(context1);
        var request2 = middleware.InvokeAsync(context2);

        await firstTwoRequestsStarted.Task;

        await middleware.InvokeAsync(context3);

        Assert.That(
            context3.Response.StatusCode,
            Is.EqualTo(StatusCodes.Status503ServiceUnavailable));

        releaseRequests.TrySetResult(true);

        await Task.WhenAll(request1, request2);
    }

    [Test]
    public async Task Should_AllowNewRequest_AfterPreviousRequestCompleted()
    {
        var nextCalls = 0;

        RequestDelegate next = _ =>
        {
            Interlocked.Increment(ref nextCalls);
            return Task.CompletedTask;
        };

        var settings = Options.Create(new Settings
        {
            ParallelLimit = 1
        });

        var middleware = new RequestLimitMiddleware(next, settings);

        var firstContext = new DefaultHttpContext();
        var secondContext = new DefaultHttpContext();

        await middleware.InvokeAsync(firstContext);
        await middleware.InvokeAsync(secondContext);

        Assert.Multiple(() =>
        {
            Assert.That(
                firstContext.Response.StatusCode,
                Is.Not.EqualTo(StatusCodes.Status503ServiceUnavailable));

            Assert.That(
                secondContext.Response.StatusCode,
                Is.Not.EqualTo(StatusCodes.Status503ServiceUnavailable));

            Assert.That(nextCalls, Is.EqualTo(2));
        });
    }
}