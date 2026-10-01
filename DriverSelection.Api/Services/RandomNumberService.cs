using System.Net.Http.Json;

namespace DriverSelection.Api.Services;

public sealed class RandomNumberService
{
    private readonly HttpClient _httpClient;

    public RandomNumberService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> GetRandomNumberAsync(
        int min,
        int maxExclusive,
        CancellationToken cancellationToken = default)
    {
        if (min >= maxExclusive)
            throw new ArgumentException("Некорректный диапазон.");

        try
        {
            var maxInclusive = maxExclusive - 1;

            var response = await _httpClient.GetAsync(
                $"http://www.randomnumberapi.com/api/v1.0/random?min={min}&max={maxInclusive}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Random.Shared.Next(min, maxExclusive);

            var numbers = await response.Content
                .ReadFromJsonAsync<int[]>(
                    cancellationToken: cancellationToken);

            if (numbers is null || numbers.Length == 0)
                return Random.Shared.Next(min, maxExclusive);

            return numbers[0];
        }
        catch
        {
            // согласно тз при ошибке удалённого api используются встроенные средства дотнет
            return Random.Shared.Next(min, maxExclusive);
        }
    }
}