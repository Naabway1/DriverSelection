using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DriverSelection.Api.Algorithms;
using DriverSelection.Api.Models;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class DriverSearchBenchmark
{
    [Params(100, 1_000, 10_000, 100_000)]
    public int DriverCount { get; set; }

    private Driver[] _drivers = [];
    private IDriverSearchAlgorithm _sorting = null!;
    private IDriverSearchAlgorithm _topFive = null!;
    private IDriverSearchAlgorithm _bucket = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(42);
        _drivers = Enumerable.Range(1, DriverCount)
            .Select(id => new Driver(id, random.Next(0, 10_000), random.Next(0, 10_000)))
            .ToArray();

        _sorting = new SortingSearchAlgorithm();
        _topFive = new TopFiveSearchAlgorithm();
        _bucket = new BucketSearchAlgorithm();
    }

    [Benchmark(Baseline = true)]
    public IReadOnlyList<DriverSearchResult> Sorting() =>
        _sorting.FindNearest(_drivers, 5_000, 5_000);

    [Benchmark]
    public IReadOnlyList<DriverSearchResult> TopFive() =>
        _topFive.FindNearest(_drivers, 5_000, 5_000);

    [Benchmark]
    public IReadOnlyList<DriverSearchResult> Bucket() =>
        _bucket.FindNearest(_drivers, 5_000, 5_000);
}
