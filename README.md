# Driver Selection

Тестовое задание: поиск пяти ближайших водителей к заказу на прямоугольной сетке.

## Технологии

- C#
- ASP.NET Core Web API
- .NET 10
- NUnit
- BenchmarkDotNet

## Реализованные алгоритмы

1. Полная сортировка всех водителей по Manhattan distance.
2. Поддержание top-5 без полной сортировки.
3. Группировка водителей по расстояниям (bucket search).

При равном расстоянии используется `Driver.Id` как tie-breaker, поэтому алгоритмы возвращают детерминированный одинаковый результат.

## Запуск API

```bash
dotnet run --project DriverSelection.Api
```

Swagger доступен на `/swagger` в Development окружении.

## Тесты

```bash
dotnet test
```

## Benchmark

```bash
dotnet run -c Release --project DriverSelection.Benchmarks
```

## Результаты BenchmarkDotNet

Для сравнения производительности алгоритмов использовался BenchmarkDotNet.
Тестирование проводилось на наборах из 100, 1 000, 10 000 и 100 000 водителей.

| Algorithm | 100 | 1 000 | 10 000 | 100 000 |
|-----------|----:|------:|-------:|--------:|
| TopFive   | 1.063 μs | 12.815 μs | 146.463 μs | 1.536 ms |
| Sorting   | 1.695 μs | 14.377 μs | 256.014 μs | 7.567 ms |
| Bucket    | 5.281 μs | 34.988 μs | 515.685 μs | 10.098 ms |

По результатам измерений алгоритм `TopFiveSearchAlgorithm` показал наименьшее
среднее время выполнения на всех размерах тестовых данных.

![Benchmark results](benchmark-results.png)