using DriverSelection.Api.Configuration;
using DriverSelection.Api.Models;
using DriverSelection.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DriverSelection.Api.Controllers;

[ApiController]
[Route("api/drivers")]
public sealed class DriversController : ControllerBase
{
    private readonly DriverStorage _storage;
    private readonly MapSettings _mapSettings;

    public DriversController(
        DriverStorage storage,
        IOptions<MapSettings> mapSettings)
    {
        _storage = storage;
        _mapSettings = mapSettings.Value;
    }

    [HttpPost("coordinates")]
    public ActionResult SetCoordinates(DriverCoordinateRequest request)
    {
        var coordinatesValid =
            request.X >= 0 &&
            request.X < _mapSettings.N &&
            request.Y >= 0 &&
            request.Y < _mapSettings.M;

        _storage.TryGet(request.Id, out var existingDriver);

        if (!coordinatesValid)
        {
            if (existingDriver is not null)
            {
                _storage.Remove(request.Id);
            }

            return BadRequest("Координаты некорректны");
        }

        if (_storage.IsPositionOccupied(
                request.X,
                request.Y,
                request.Id))
        {
            return BadRequest("Здесь уже находится другой водитель");
        }

        var driver = new Driver(
            request.Id,
            request.X,
            request.Y);

        _storage.Add(driver);

        return Ok(
            existingDriver is null
                ? "Координаты успешно добавлены"
                : "Координаты успешно изменены");
    }
}