using DriverSelection.Api.Models;

namespace DriverSelection.Api.Algorithms;

public sealed class BucketSearchAlgorithm : IDriverSearchAlgorithm
{
    public IReadOnlyList<DriverSearchResult> FindNearest(
        IReadOnlyCollection<Driver> drivers,
        int orderX,
        int orderY,
        int count = 5)
    {
        if (count <= 0 || drivers.Count == 0)
            return [];

        var buckets = new Dictionary<int, List<DriverSearchResult>>();
        var maxDistance = 0;

        foreach (var driver in drivers)
        {
            var result = DistanceCalculator.ToResult(driver, orderX, orderY);
            maxDistance = Math.Max(maxDistance, result.Distance);

            if (!buckets.TryGetValue(result.Distance, out var bucket))
            {
                bucket = [];
                buckets[result.Distance] = bucket;
            }

            bucket.Add(result);
        }

        var resultList = new List<DriverSearchResult>(Math.Min(count, drivers.Count));

        for (var distance = 0; distance <= maxDistance && resultList.Count < count; distance++)
        {
            if (!buckets.TryGetValue(distance, out var bucket))
                continue;

            bucket.Sort(static (left, right) => left.Id.CompareTo(right.Id));

            foreach (var driver in bucket)
            {
                resultList.Add(driver);

                if (resultList.Count == count)
                    break;
            }
        }

        return resultList;
    }
}
