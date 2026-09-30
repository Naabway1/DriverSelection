using DriverSelection.Api.Models;

namespace DriverSelection.Api.Algorithms;

public sealed class SortingSearchAlgorithm : IDriverSearchAlgorithm
{
    public IReadOnlyList<DriverSearchResult> FindNearest(
        IReadOnlyCollection<Driver> drivers,
        int orderX,
        int orderY,
        int count = 5)
    {
        if (count <= 0 || drivers.Count == 0)
            return [];

        return drivers
            .Select(driver => DistanceCalculator.ToResult(driver, orderX, orderY))
            .OrderBy(result => result.Distance)
            .ThenBy(result => result.Id)
            .Take(count)
            .ToList();
    }
}
