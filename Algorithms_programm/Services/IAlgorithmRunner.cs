namespace Algorithms_programm.Services;

/// <summary>
/// Единый, не-generic фасад над любым алгоритмом (ITimedAlgorithm&lt;TInput,TResult&gt; или
/// IStepCountingAlgorithm&lt;TInput,TResult&gt;), нужен, чтобы AlgorithmRegistry/ExperimentOrchestrator
/// и GUI могли работать со списком алгоритмов единообразно, не зная и не заботясь о том, что
/// у SumAlgorithm вход double[], у MatrixMultiplicationAlgorithm — MatrixPair, у FastPowerAlgorithm —
/// PowerInput и т.д. Конкретные реализации — TimedAlgorithmRunner&lt;TInput,TResult&gt; и
/// StepCountingAlgorithmRunner&lt;TInput,TResult&gt; в этой же папке.
/// </summary>
public interface IAlgorithmRunner
{
    string Name { get; }
    AlgorithmCategory Category { get; }
    int DefaultNMax { get; }

    /// <summary>True для алгоритмов Части IV — измеряется число шагов, а не время.</summary>
    bool IsStepCounting { get; }

    /// <summary>True только для матричного умножения — вход двумерный (n, m).</summary>
    bool IsMatrix { get; }

    /// <summary>
    /// runsPerPoint независимых замеров времени (мс) при заданном n (и m для матриц).
    /// Бросает NotSupportedException, если IsStepCounting == true.
    /// </summary>
    Task<IReadOnlyList<double>> MeasureTimeRunsAsync(int n, int runsPerPoint, int? m, CancellationToken ct = default);

    /// <summary>
    /// runsPerPoint независимых замеров числа шагов при заданном n.
    /// Бросает NotSupportedException, если IsStepCounting == false.
    /// </summary>
    Task<IReadOnlyList<long>> MeasureStepRunsAsync(int n, int runsPerPoint, CancellationToken ct = default);
}
