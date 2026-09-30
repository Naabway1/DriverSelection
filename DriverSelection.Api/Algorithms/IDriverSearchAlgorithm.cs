using DriverSelection.Api.Models;

namespace DriverSelection.Api.Algorithms;

public interface IDriverSearchAlgorithm
{
    IReadOnlyList<DriverSearchResult> FindNearest(
        IReadOnlyCollection<Driver> drivers,
        int orderX,
        int orderY,
        int count = 5);
}
