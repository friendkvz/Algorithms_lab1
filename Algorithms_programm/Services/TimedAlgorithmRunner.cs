using Algorithms_programm.Algorithms.Abstractions;
using Algorithms_programm.Benchmarking;

namespace Algorithms_programm.Services;

/// <summary>
/// Обёртка над ITimedAlgorithm&lt;TInput,TResult&gt;. inputFactory получает n и (для матриц) m
/// и должен вернуть свежесгенерированный вход — вызывается заново на каждый из runsPerPoint
/// запусков (методичка: "5 независимых запусков").
/// </summary>
public sealed class TimedAlgorithmRunner<TInput, TResult> : IAlgorithmRunner
{
    private readonly ITimedAlgorithm<TInput, TResult> _algorithm;
    private readonly Func<int, int?, TInput> _inputFactory;
    private readonly TimeBenchmarkRunner _benchmarkRunner = new();

    public TimedAlgorithmRunner(
        ITimedAlgorithm<TInput, TResult> algorithm,
        AlgorithmCategory category,
        int defaultNMax,
        Func<int, int?, TInput> inputFactory,
        bool isMatrix = false)
    {
        _algorithm = algorithm;
        _inputFactory = inputFactory;
        Category = category;
        DefaultNMax = defaultNMax;
        IsMatrix = isMatrix;
    }

    public string Name => _algorithm.Name;
    public AlgorithmCategory Category { get; }
    public int DefaultNMax { get; }
    public bool IsStepCounting => false;
    public bool IsMatrix { get; }

    public Task<IReadOnlyList<double>> MeasureTimeRunsAsync(
        int n, int runsPerPoint, int? m, CancellationToken ct = default)
    {
        return _benchmarkRunner.MeasureMillisecondsAsync(
            _algorithm, () => _inputFactory(n, m), runsPerPoint, ct);
    }

    public Task<IReadOnlyList<long>> MeasureStepRunsAsync(int n, int runsPerPoint, CancellationToken ct = default)
    {
        throw new NotSupportedException($"Алгоритм '{Name}' измеряет время, а не число шагов.");
    }
}
