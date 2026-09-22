using Algorithms_programm.Algorithms.Abstractions;
using Algorithms_programm.Benchmarking;

namespace Algorithms_programm.Services;

/// <summary>Обёртка над IStepCountingAlgorithm&lt;TInput,TResult&gt; — алгоритмы Части IV (возведение в степень).</summary>
public sealed class StepCountingAlgorithmRunner<TInput, TResult> : IAlgorithmRunner
{
    private readonly IStepCountingAlgorithm<TInput, TResult> _algorithm;
    private readonly Func<int, TInput> _inputFactory;
    private readonly StepCountBenchmarkRunner _benchmarkRunner = new();

    public StepCountingAlgorithmRunner(
        IStepCountingAlgorithm<TInput, TResult> algorithm,
        int defaultNMax,
        Func<int, TInput> inputFactory)
    {
        _algorithm = algorithm;
        _inputFactory = inputFactory;
        DefaultNMax = defaultNMax;
    }

    public string Name => _algorithm.Name;
    public AlgorithmCategory Category => AlgorithmCategory.Power;
    public int DefaultNMax { get; }
    public bool IsStepCounting => true;
    public bool IsMatrix => false;

    public Task<IReadOnlyList<double>> MeasureTimeRunsAsync(
        int n, int runsPerPoint, int? m, CancellationToken ct = default)
    {
        throw new NotSupportedException($"Алгоритм '{Name}' измеряет число шагов, а не время.");
    }

    public Task<IReadOnlyList<long>> MeasureStepRunsAsync(int n, int runsPerPoint, CancellationToken ct = default)
    {
        return _benchmarkRunner.MeasureStepsAsync(_algorithm, () => _inputFactory(n), runsPerPoint, ct);
    }
}
