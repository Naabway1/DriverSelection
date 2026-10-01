using DriverSelection.Api.Models;

namespace DriverSelection.Api.Services;

public sealed class DriverStorage
{
    private readonly Dictionary<int, Driver> _drivers = new();
    private readonly object _lock = new();

    public IReadOnlyCollection<Driver> GetAll()
    {
        lock (_lock)
        {
            return _drivers.Values.ToArray();
        }
    }

    public bool TryGet(int id, out Driver? driver)
    {
        lock (_lock)
        {
            return _drivers.TryGetValue(id, out driver);
        }
    }

    public bool IsPositionOccupied(int x, int y, int exceptDriverId = -1)
    {
        lock (_lock)
        {
            return _drivers.Values.Any(d =>
                d.Id != exceptDriverId &&
                d.X == x &&
                d.Y == y);
        }
    }

    public void Add(Driver driver)
    {
        lock (_lock)
        {
            _drivers[driver.Id] = driver;
        }
    }

    public void Remove(int id)
    {
        lock (_lock)
        {
            _drivers.Remove(id);
        }
    }

    public bool TrySetCoordinates(
    int driverId,
    int x,
    int y,
    out bool existed,
    out bool occupied)
    {
        lock (_lock)
        {
            existed = _drivers.ContainsKey(driverId);

            occupied = _drivers.Values.Any(d =>
                d.Id != driverId &&
                d.X == x &&
                d.Y == y);

            if (occupied)
            {
                return false;
            }

            _drivers[driverId] = new Driver(driverId, x, y);

            return true;
        }
    }
}