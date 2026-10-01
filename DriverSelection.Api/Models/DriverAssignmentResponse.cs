
namespace DriverSelection.Api.Models
{
    public sealed record DriverAssignmentResponse(
    int Id,
    int X,
    int Y,
    int RouteLength,
    IReadOnlyList<Coordinate> Route);
}
