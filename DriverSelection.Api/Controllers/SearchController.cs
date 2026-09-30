using DriverSelection.Api.Algorithms;
using DriverSelection.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace DriverSelection.Api.Controllers;

[ApiController]
[Route("api/search")]
public sealed class SearchController : ControllerBase
{
    [HttpPost("compare")]
    public ActionResult<object> Compare([FromBody] SearchRequest request)
    {
        var sorting = new SortingSearchAlgorithm();
        var topFive = new TopFiveSearchAlgorithm();
        var bucket = new BucketSearchAlgorithm();

        return Ok(new
        {
            sorting = sorting.FindNearest(request.Drivers, request.OrderX, request.OrderY),
            topFive = topFive.FindNearest(request.Drivers, request.OrderX, request.OrderY),
            bucket = bucket.FindNearest(request.Drivers, request.OrderX, request.OrderY)
        });
    }
}
