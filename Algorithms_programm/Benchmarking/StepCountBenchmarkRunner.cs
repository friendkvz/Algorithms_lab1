using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Benchmarking;

/// <summary>
/// Замер количества операций для алгоритмов Части IV методички (возведение в степень):
/// здесь измеряется не время, а число шагов (умножений), которое возвращает сам алгоритм
/// через IStepCountingAlgorithm.ExecuteCountingSteps. Формально шаги детерминированы при
/// фиксированных (x, n) — randomness здесь только в том, что x генерируется заново на каждый
/// запуск (как и для остальных алгоритмов), но число шагов от значения x не зависит, оно
/// зависит только от n. Несколько запусков на точку тем не менее выполняются — для единообразия
/// с остальными алгоритмами и на случай, если в будущем алгоритм Части III (custom) с похожим
/// интерфейсом будет step-count-зависим от входных данных, а не только от n.
/// </summary>
public sealed class StepCountBenchmarkRunner
{
    public IReadOnlyList<long> MeasureSteps<TInput, TResult>(
        IStepCountingAlgorithm<TInput, TResult> algorithm,
        Func<TInput> inputFactory,
        int runsPerPoint)
    {
        if (runsPerPoint < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(runsPerPoint), "Число запусков должно быть >= 1.");
        }

        var results = new List<long>(runsPerPoint);
        for (var i = 0; i < runsPerPoint; i++)
        {
            var input = inputFactory();
            var (_, steps) = algorithm.ExecuteCountingSteps(input);
            results.Add(steps);
        }

        return results;
    }

    public Task<IReadOnlyList<long>> MeasureStepsAsync<TInput, TResult>(
        IStepCountingAlgorithm<TInput, TResult> algorithm,
        Func<TInput> inputFactory,
        int runsPerPoint,
        CancellationToken ct = default)
    {
        return Task.Run(() => MeasureSteps(algorithm, inputFactory, runsPerPoint), ct);
    }

    public static double Average(IReadOnlyList<long> measurements) =>
        measurements.Count == 0 ? 0 : measurements.Average();
}
