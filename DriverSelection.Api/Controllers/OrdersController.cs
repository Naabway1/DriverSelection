using DriverSelection.Api.Algorithms;
using DriverSelection.Api.Configuration;
using DriverSelection.Api.Models;
using DriverSelection.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DriverSelection.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly DriverStorage _storage;
    private readonly MapSettings _mapSettings;
    private readonly RandomNumberService _randomNumberService;

    public OrdersController(
        DriverStorage storage,
        IOptions<MapSettings> mapSettings,
        RandomNumberService randomNumberService)
    {
        _storage = storage;
        _mapSettings = mapSettings.Value;
        _randomNumberService = randomNumberService;
    }

    [HttpPost("select-driver")]
    public async Task<ActionResult<DriverAssignmentResponse>> SelectDriver(
        OrderRequest request,
        CancellationToken cancellationToken)
    {
        if (request.X < 0 ||
            request.X >= _mapSettings.N ||
            request.Y < 0 ||
            request.Y >= _mapSettings.M)
        {
            return BadRequest("Координаты некорректны");
        }

        var drivers = _storage.GetAll();

        if (drivers.Count == 0)
        {
            return BadRequest("Свободных водителей нет");
        }

        // лучший алгоритм из первой части 
        var algorithm = new TopFiveSearchAlgorithm();

        var nearestDrivers = algorithm.FindNearest(
            drivers,
            request.X,
            request.Y,
            5);

        if (nearestDrivers.Count == 0)
        {
            return BadRequest("Свободных водителей нет");
        }

        var randomIndex = await _randomNumberService.GetRandomNumberAsync(
            0,
            nearestDrivers.Count,
            cancellationToken);

        var selectedDriver = nearestDrivers[randomIndex];

        var route = BuildRoute(
            selectedDriver.X,
            selectedDriver.Y,
            request.X,
            request.Y);

        var result = new DriverAssignmentResponse(
            selectedDriver.Id,
            selectedDriver.X,
            selectedDriver.Y,
            route.Count - 1,
            route);

        return Ok(result);
    }

    private static List<Coordinate> BuildRoute(
        int startX,
        int startY,
        int targetX,
        int targetY)
    {
        var route = new List<Coordinate>
        {
            new(startX, startY)
        };

        var x = startX;
        var y = startY;

        while (x != targetX)
        {
            x += x < targetX ? 1 : -1;
            route.Add(new Coordinate(x, y));
        }

        while (y != targetY)
        {
            y += y < targetY ? 1 : -1;
            route.Add(new Coordinate(x, y));
        }

        return route;
    }
}