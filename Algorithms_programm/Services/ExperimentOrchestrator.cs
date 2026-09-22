using Algorithms_programm.Approximation;
using Algorithms_programm.Benchmarking;
using Algorithms_programm.Caching;
using Algorithms_programm.Database.Entities;
using Algorithms_programm.Models;

namespace Algorithms_programm.Services;

/// <summary>
/// Главная точка входа для GUI: "склеивает генерацию → бенчмарк → кэш → БД → аппроксимацию
/// в один вызов" (Services/ExperimentOrchestrator из требований). ViewModel вызывает один из
/// двух методов (обычный или матричный) и получает готовый DTO с точками и аппроксимацией,
/// не зная ничего о кэшировании, EF Core или конкретных типах алгоритмов.
/// </summary>
public sealed class ExperimentOrchestrator
{
    private readonly AlgorithmRegistry _registry;
    private readonly ICacheService _cache;

    public ExperimentOrchestrator(AlgorithmRegistry registry, ICacheService cache)
    {
        _registry = registry;
        _cache = cache;
    }

    public AlgorithmRegistry Registry => _registry;

    /// <summary>
    /// Полный цикл эксперимента для любого не-матричного алгоритма (Части I, III, IV методички).
    /// progress сообщает долю выполненных точек n в диапазоне [0, 1] — предназначен для
    /// прогресс-бара в GUI, вызывается из фонового потока (см. TimeBenchmarkRunner/
    /// StepCountBenchmarkRunner — сами замеры выполняются через Task.Run).
    /// </summary>
    public async Task<ExperimentResultDto> RunExperimentAsync(
        string algorithmName,
        int nMax,
        int step,
        int runsPerPoint,
        bool forceRecalculate,
        IProgress<double>? progress = null,
        CancellationToken ct = default)
    {
        var runner = _registry.Get(algorithmName);
        if (runner.IsMatrix)
        {
            throw new InvalidOperationException(
                $"'{algorithmName}' — матричный алгоритм с двумя размерностями (n, m), используйте RunMatrixExperimentAsync.");
        }

        var config = new ExperimentConfig(algorithmName, nMax, step, runsPerPoint);
        var sessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate, ct);

        var nValues = config.EnumerateN().ToList();
        var points = new List<EmpiricalPointDto>(nValues.Count);

        for (var i = 0; i < nValues.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            var n = nValues[i];

            var point = await MeasureOrReusePointAsync(runner, sessionId, n, m: null, runsPerPoint, ct);
            points.Add(point);

            progress?.Report((i + 1) / (double)nValues.Count);
        }

        var dataPoints = points.Select(p => new DataPoint(p.N, p.MeanValue)).ToList();
        var approximations = FunctionApproximator.ApproximateAll(dataPoints);

        return new ExperimentResultDto(algorithmName, sessionId, points, approximations);
    }

    /// <summary>
    /// Полный цикл эксперимента для матричного умножения (алгоритм 8): перебирает 2D-сетку
    /// (n, m) — по методичке нужен график время(n, m). Аппроксимация теоретической функцией
    /// здесь не строится (методичка просит только сам 3D/heatmap-график для этого случая).
    /// </summary>
    public async Task<MatrixExperimentResultDto> RunMatrixExperimentAsync(
        string algorithmName,
        int nMax,
        int step,
        int mMax,
        int mStep,
        int runsPerPoint,
        bool forceRecalculate,
        IProgress<double>? progress = null,
        CancellationToken ct = default)
    {
        var runner = _registry.Get(algorithmName);
        if (!runner.IsMatrix)
        {
            throw new InvalidOperationException($"'{algorithmName}' — не матричный алгоритм.");
        }

        var config = new ExperimentConfig(algorithmName, nMax, step, runsPerPoint, mMax, mStep);
        var sessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate, ct);

        var nValues = config.EnumerateN().ToList();
        var mValues = ExperimentConfig.EnumerateRange(mMax, mStep).ToList();
        var totalPoints = nValues.Count * mValues.Count;
        var points = new List<EmpiricalPointDto>(totalPoints);
        var completed = 0;

        foreach (var n in nValues)
        {
            foreach (var m in mValues)
            {
                ct.ThrowIfCancellationRequested();

                var point = await MeasureOrReusePointAsync(runner, sessionId, n, m, runsPerPoint, ct);
                points.Add(point);

                completed++;
                progress?.Report(completed / (double)totalPoints);
            }
        }

        return new MatrixExperimentResultDto(sessionId, points);
    }

    /// <summary>
    /// Проверяет кэш для точки (n, m); если данных достаточно — берёт среднее из БД, иначе
    /// запускает бенчмарк, сохраняет "сырые" запуски в БД и возвращает среднее по ним.
    /// Единая логика и для времени, и для числа шагов — определяется по runner.IsStepCounting.
    /// </summary>
    private async Task<EmpiricalPointDto> MeasureOrReusePointAsync(
        IAlgorithmRunner runner, int sessionId, int n, int? m, int runsPerPoint, CancellationToken ct)
    {
        var kind = runner.IsStepCounting ? MeasureKind.StepCount : MeasureKind.ElapsedMilliseconds;

        if (await _cache.IsPointCachedAsync(sessionId, n, runsPerPoint, m, ct))
        {
            var cachedRuns = await _cache.GetCachedRunsAsync(sessionId, n, m, ct);
            var cachedMean = kind == MeasureKind.StepCount
                ? cachedRuns.Average(r => (double)(r.StepCount ?? 0))
                : cachedRuns.Average(r => r.ElapsedMilliseconds ?? 0);

            return new EmpiricalPointDto(n, m, cachedMean, cachedRuns.Count, kind);
        }

        if (runner.IsStepCounting)
        {
            var steps = await runner.MeasureStepRunsAsync(n, runsPerPoint, ct);
            var entities = steps.Select((value, idx) => new ExperimentRunEntity
            {
                N = n,
                M = m,
                RunIndex = idx + 1,
                StepCount = value,
                MeasuredAt = DateTime.UtcNow,
            });
            await _cache.SaveRunsAsync(sessionId, entities, ct);

            return new EmpiricalPointDto(n, m, steps.Average(v => (double)v), steps.Count, kind);
        }
        else
        {
            var times = await runner.MeasureTimeRunsAsync(n, runsPerPoint, m, ct);
            var entities = times.Select((value, idx) => new ExperimentRunEntity
            {
                N = n,
                M = m,
                RunIndex = idx + 1,
                ElapsedMilliseconds = value,
                MeasuredAt = DateTime.UtcNow,
            });
            await _cache.SaveRunsAsync(sessionId, entities, ct);

            return new EmpiricalPointDto(n, m, times.Average(), times.Count, kind);
        }
    }
}
