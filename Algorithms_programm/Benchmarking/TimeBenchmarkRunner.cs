using System.Diagnostics;
using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Benchmarking;

/// <summary>
/// Замер времени выполнения для алгоритмов Частей I–III методички: runsPerPoint независимых
/// запусков, каждый — на свежесгенерированных случайных входных данных (по методичке "5
/// независимых запусков"), время каждого — через Stopwatch. Среднее не считается здесь —
/// это осознанно вынесено наружу (Services/ExperimentOrchestrator на следующем этапе),
/// потому что "сырые" замеры нужно сохранить в БД по отдельности (см. ExperimentRunEntity).
/// </summary>
public sealed class TimeBenchmarkRunner
{
    /// <summary>
    /// Выполняет runsPerPoint независимых запусков algorithm.Execute(...) на входах,
    /// сгенерированных inputFactory() заново на каждый запуск, и возвращает время каждого
    /// запуска в миллисекундах, в порядке выполнения (индекс + 1 = "номер запуска").
    /// </summary>
    public IReadOnlyList<double> MeasureMilliseconds<TInput, TResult>(
        ITimedAlgorithm<TInput, TResult> algorithm,
        Func<TInput> inputFactory,
        int runsPerPoint)
    {
        if (runsPerPoint < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(runsPerPoint), "Число запусков должно быть >= 1.");
        }

        var results = new List<double>(runsPerPoint);
        for (var i = 0; i < runsPerPoint; i++)
        {
            var input = inputFactory();

            var stopwatch = Stopwatch.StartNew();
            algorithm.Execute(input);
            stopwatch.Stop();

            results.Add(stopwatch.Elapsed.TotalMilliseconds);
        }

        return results;
    }

    /// <summary>
    /// Асинхронная обёртка: сам бенчмарк остаётся синхронным CPU-bound кодом (Stopwatch
    /// вокруг обычного вызова), но выполняется на пуле потоков через Task.Run, чтобы вызывающий
    /// UI-поток WPF не блокировался на потенциально многосекундном замере (Этап 2/4 методички:
    /// "не блокировать UI-поток при долгих вычислениях").
    /// </summary>
    public Task<IReadOnlyList<double>> MeasureMillisecondsAsync<TInput, TResult>(
        ITimedAlgorithm<TInput, TResult> algorithm,
        Func<TInput> inputFactory,
        int runsPerPoint,
        CancellationToken ct = default)
    {
        return Task.Run(() => MeasureMilliseconds(algorithm, inputFactory, runsPerPoint), ct);
    }

    public static double Average(IReadOnlyList<double> measurements) =>
        measurements.Count == 0 ? 0 : measurements.Average();
}
