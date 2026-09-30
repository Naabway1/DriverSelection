using DriverSelection.Api.Algorithms;
using DriverSelection.Api.Models;
using NUnit.Framework;

namespace DriverSelection.Tests;

public class AlgorithmTests
{
    private static readonly IDriverSearchAlgorithm[] Algorithms =
    [
        new SortingSearchAlgorithm(),
        new TopFiveSearchAlgorithm(),
        new BucketSearchAlgorithm()
    ];

    [Test]
    public void AllAlgorithms_ShouldReturnSameFiveDrivers()
    {
        var drivers = Enumerable.Range(1, 20)
            .Select(i => new Driver(i, i % 7, i % 5))
            .ToArray();

        var results = Algorithms
            .Select(a => a.FindNearest(drivers, 3, 2).Select(x => x.Id).ToArray())
            .ToArray();

        Assert.That(results[1], Is.EqualTo(results[0]));
        Assert.That(results[2], Is.EqualTo(results[0]));
    }

    [Test]
    public void ShouldReturnLessThanFive_WhenThereAreLessThanFiveDrivers()
    {
        var drivers = new[]
        {
            new Driver(1, 0, 0),
            new Driver(2, 1, 1),
            new Driver(3, 5, 5)
        };

        foreach (var algorithm in Algorithms)
        {
            var result = algorithm.FindNearest(drivers, 0, 0);
            Assert.That(result, Has.Count.EqualTo(3));
        }
    }

    [Test]
    public void ShouldReturnEmpty_WhenThereAreNoDrivers()
    {
        foreach (var algorithm in Algorithms)
        {
            var result = algorithm.FindNearest([], 0, 0);
            Assert.That(result, Is.Empty);
        }
    }

    [Test]
    public void ShouldUseIdAsTieBreaker()
    {
        var drivers = new[]
        {
            new Driver(3, 1, 0),
            new Driver(1, 0, 1),
            new Driver(2, -1, 0),
            new Driver(4, 0, -1),
            new Driver(5, 2, 0),
            new Driver(6, 3, 0)
        };

        var expected = new[] { 1, 2, 3, 4, 5 };

        foreach (var algorithm in Algorithms)
        {
            var actual = algorithm.FindNearest(drivers, 0, 0).Select(x => x.Id).ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    [Test]
    public void Distance_ShouldBeManhattanDistance()
    {
        var driver = new Driver(1, 2, 3);
        var result = new SortingSearchAlgorithm().FindNearest([driver], 7, 6);

        Assert.That(result[0].Distance, Is.EqualTo(8));
    }
}
