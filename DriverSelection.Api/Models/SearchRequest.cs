namespace DriverSelection.Api.Models;

public sealed class SearchRequest
{
    public int OrderX { get; init; }
    public int OrderY { get; init; }
    public List<Driver> Drivers { get; init; } = [];
}
