using DriverSelection.Api.Models;

namespace DriverSelection.Api.Algorithms;

public static class DistanceCalculator
{
    public static int Manhattan(Driver driver, int orderX, int orderY) =>
        Math.Abs(driver.X - orderX) + Math.Abs(driver.Y - orderY);

    public static DriverSearchResult ToResult(Driver driver, int orderX, int orderY) =>
        new(driver.Id, driver.X, driver.Y, Manhattan(driver, orderX, orderY));
}
