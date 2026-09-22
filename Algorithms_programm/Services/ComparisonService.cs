using Algorithms_programm.Database.Repositories;
using Algorithms_programm.Models;

namespace Algorithms_programm.Services;

/// <summary>
/// Выборка истории для окна сравнения нескольких экспериментов на одном графике (Этап 4
/// методички). В отличие от ExperimentOrchestrator, ничего не запускает и не кэширует —
/// только читает то, что уже сохранено в БД.
/// </summary>
public sealed class ComparisonService
{
    private readonly IExperimentRepository _repository;

    public ComparisonService(IExperimentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SessionSummaryDto>> GetSessionsAsync(
        string algorithmName, CancellationToken ct = default)
    {
        var sessions = await _repository.GetSessionsForAlgorithmAsync(algorithmName, ct);

        return sessions
            .Select(s => new SessionSummaryDto(
                s.Id, s.Algorithm.Name, s.CreatedAt, s.NMax, s.Step, s.RunsPerPoint, s.MMax, s.MStep, s.Label))
            .ToList();
    }

    /// <summary>
    /// Усреднённые точки сессии, готовые к отрисовке (каждая (n, m) точка усредняется по всем
    /// сохранённым для неё запускам). Kind определяется по наличию StepCount vs ElapsedMilliseconds
    /// среди самих запусков — сессия целиком либо про время, либо про шаги, никогда не смешивает оба.
    /// </summary>
    public async Task<IReadOnlyList<EmpiricalPointDto>> GetSessionPointsAsync(int sessionId, CancellationToken ct = default)
    {
        var runs = await _repository.GetAllRunsForSessionAsync(sessionId, ct);
        if (runs.Count == 0)
        {
            return Array.Empty<EmpiricalPointDto>();
        }

        var isStepCounting = runs[0].StepCount.HasValue;
        var kind = isStepCounting ? MeasureKind.StepCount : MeasureKind.ElapsedMilliseconds;

        return runs
            .GroupBy(r => (r.N, r.M))
            .Select(group =>
            {
                var mean = isStepCounting
                    ? group.Average(r => (double)(r.StepCount ?? 0))
                    : group.Average(r => r.ElapsedMilliseconds ?? 0);

                return new EmpiricalPointDto(group.Key.N, group.Key.M, mean, group.Count(), kind);
            })
            .OrderBy(p => p.N)
            .ThenBy(p => p.M)
            .ToList();
    }
}
