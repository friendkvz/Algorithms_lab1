using Algorithms_programm.Benchmarking;
using Algorithms_programm.Database.Entities;
using Algorithms_programm.Database.Repositories;

namespace Algorithms_programm.Caching;

/// <summary>
/// Реализация кэша поверх БД: "есть ли уже данные для (алгоритм, n, конфигурация
/// эксперимента) в БД — использовать их вместо повторного запуска" (Этап 2 методички).
/// Единицей кэша является ExperimentSessionEntity (конфигурация целиком: N_max/Step/RunsPerPoint
/// и, для матриц, M_max/M_step), а не отдельная точка n — но проверка "нужно ли досчитывать"
/// всё равно делается по каждой точке (n, m) отдельно через IsPointCachedAsync, потому что
/// одна и та же сессия может быть досчитана позже (например, если раньше эксперимент прервали).
/// </summary>
public sealed class DbBackedCacheService : ICacheService
{
    private readonly IExperimentRepository _repository;

    public DbBackedCacheService(IExperimentRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> GetOrCreateSessionAsync(
        ExperimentConfig config, bool forceRecalculate, CancellationToken ct = default)
    {
        var configHash = config.ComputeConfigHash();
        var existing = await _repository.FindSessionByConfigAsync(config.AlgorithmName, configHash, ct);

        if (existing is not null)
        {
            if (!forceRecalculate)
            {
                return existing.Id;
            }

            // Принудительный пересчёт: старые данные для этой конфигурации удаляются целиком,
            // чтобы не смешивать в одном графике старые и новые точки одной и той же сессии.
            await _repository.DeleteSessionAsync(existing.Id, ct);
        }

        var created = await _repository.CreateSessionAsync(
            algorithmName: config.AlgorithmName,
            nMax: config.NMax,
            step: config.Step,
            runsPerPoint: config.RunsPerPoint,
            configHash: configHash,
            mMax: config.MMax,
            mStep: config.MStep,
            ct: ct);

        return created.Id;
    }

    public Task<IReadOnlyList<ExperimentRunEntity>> GetCachedRunsAsync(
        int sessionId, int n, int? m = null, CancellationToken ct = default)
    {
        return GetCachedRunsInternalAsync(sessionId, n, m, ct);
    }

    private async Task<IReadOnlyList<ExperimentRunEntity>> GetCachedRunsInternalAsync(
        int sessionId, int n, int? m, CancellationToken ct)
    {
        var runs = await _repository.GetRunsAsync(sessionId, n, m, ct);
        return runs;
    }

    public async Task<bool> IsPointCachedAsync(
        int sessionId, int n, int requiredRuns, int? m = null, CancellationToken ct = default)
    {
        var runs = await _repository.GetRunsAsync(sessionId, n, m, ct);
        return runs.Count >= requiredRuns;
    }

    public async Task SaveRunsAsync(
        int sessionId,
        IEnumerable<ExperimentRunEntity> runs,
        CancellationToken ct = default)
    {
        if (sessionId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sessionId),
                sessionId,
                "Идентификатор сессии должен быть положительным.");
        }

        var runList = runs.ToList();

        foreach (var run in runList)
        {
            run.SessionId = sessionId;
        }

        await _repository.AddRunsAsync(runList, ct);
    }
}
