using DriverSelection.Api.Models;

namespace DriverSelection.Api.Algorithms;

public sealed class TopFiveSearchAlgorithm : IDriverSearchAlgorithm
{
    public IReadOnlyList<DriverSearchResult> FindNearest(
        IReadOnlyCollection<Driver> drivers,
        int orderX,
        int orderY,
        int count = 5)
    {
        if (count <= 0 || drivers.Count == 0)
            return [];

        var best = new List<DriverSearchResult>(count);

        foreach (var driver in drivers)
        {
            var candidate = DistanceCalculator.ToResult(driver, orderX, orderY);
            var insertIndex = FindInsertIndex(best, candidate);

            if (insertIndex >= count)
                continue;

            best.Insert(insertIndex, candidate);

            if (best.Count > count)
                best.RemoveAt(best.Count - 1);
        }

        return best;
    }

    private static int FindInsertIndex(
        IReadOnlyList<DriverSearchResult> items,
        DriverSearchResult candidate)
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (Compare(candidate, items[i]) < 0)
                return i;
        }

        return items.Count;
    }

    private static int Compare(DriverSearchResult left, DriverSearchResult right)
    {
        var distanceComparison = left.Distance.CompareTo(right.Distance);
        return distanceComparison != 0
            ? distanceComparison
            : left.Id.CompareTo(right.Id);
    }
}
