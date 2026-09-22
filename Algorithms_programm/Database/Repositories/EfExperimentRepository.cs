using Algorithms_programm.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Algorithms_programm.Database.Repositories;

/// <summary>
/// EF Core repository for experiment data.
/// </summary>
public sealed class EfExperimentRepository : IExperimentRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public EfExperimentRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<AlgorithmEntity> GetOrCreateAlgorithmAsync(
        string algorithmName,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        var algorithm = await context.Algorithms
            .SingleOrDefaultAsync(a => a.Name == algorithmName, ct);

        if (algorithm is not null)
        {
            return algorithm;
        }

        algorithm = new AlgorithmEntity { Name = algorithmName };
        context.Algorithms.Add(algorithm);
        await context.SaveChangesAsync(ct);
        return algorithm;
    }

    public async Task<ExperimentSessionEntity?> FindSessionByConfigAsync(
        string algorithmName,
        string configHash,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        return await context.ExperimentSessions
            .Include(s => s.Algorithm)
            .Where(s => s.Algorithm.Name == algorithmName && s.ConfigHash == configHash)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<ExperimentSessionEntity> CreateSessionAsync(
        string algorithmName,
        int nMax,
        int step,
        int runsPerPoint,
        string configHash,
        int? mMax = null,
        int? mStep = null,
        string? label = null,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        var algorithm = await context.Algorithms
            .SingleOrDefaultAsync(a => a.Name == algorithmName, ct);

        if (algorithm is null)
        {
            algorithm = new AlgorithmEntity { Name = algorithmName };
            context.Algorithms.Add(algorithm);
            await context.SaveChangesAsync(ct);
        }

        if (algorithm.Id <= 0)
        {
            throw new InvalidOperationException(
                $"Не удалось получить идентификатор алгоритма '{algorithmName}'.");
        }

        var session = new ExperimentSessionEntity
        {
            AlgorithmId = algorithm.Id,
            CreatedAt = DateTime.UtcNow,
            NMax = nMax,
            Step = step,
            RunsPerPoint = runsPerPoint,
            MMax = mMax,
            MStep = mStep,
            ConfigHash = configHash,
            Label = label,
        };

        context.ExperimentSessions.Add(session);
        await context.SaveChangesAsync(ct);

        // Явно перечитываем строку из БД. Это гарантирует, что возвращаемый DTO
        // содержит сгенерированный SQLite ключ, а не значение Id по умолчанию (0).
        var savedSession = await context.ExperimentSessions
            .AsNoTracking()
            .Include(s => s.Algorithm)
            .SingleAsync(s => s.Id == session.Id, ct);

        if (savedSession.Id <= 0)
        {
            throw new InvalidOperationException(
                $"SQLite не вернул корректный идентификатор созданной сессии '{algorithmName}'.");
        }

        return savedSession;
    }

    public async Task AddRunsAsync(
        IEnumerable<ExperimentRunEntity> runs,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        var sourceRuns = runs.ToList();

        if (sourceRuns.Count == 0)
        {
            return;
        }

        var sessionIds = sourceRuns
            .Select(r => r.SessionId)
            .Distinct()
            .ToArray();

        if (sessionIds.Length != 1 || sessionIds[0] <= 0)
        {
            throw new InvalidOperationException(
                $"Некорректный идентификатор сессии: {string.Join(", ", sessionIds)}.");
        }

        var sessionId = sessionIds[0];
        var session = await context.ExperimentSessions
            .SingleOrDefaultAsync(s => s.Id == sessionId, ct);

        if (session is null)
        {
            throw new InvalidOperationException(
                $"Сессия эксперимента с ID {sessionId} не найдена.");
        }

        var entities = sourceRuns.Select(run => new ExperimentRunEntity
        {
            SessionId = session.Id,
            Session = session,
            N = run.N,
            M = run.M,
            RunIndex = run.RunIndex,
            ElapsedMilliseconds = run.ElapsedMilliseconds,
            StepCount = run.StepCount,
            MeasuredAt = run.MeasuredAt,
        }).ToList();

        context.ExperimentRuns.AddRange(entities);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<ExperimentRunEntity>> GetRunsAsync(
        int sessionId,
        int n,
        int? m = null,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        return await context.ExperimentRuns
            .Where(r => r.SessionId == sessionId && r.N == n && r.M == m)
            .ToListAsync(ct);
    }

    public async Task<List<ExperimentRunEntity>> GetAllRunsForSessionAsync(
        int sessionId,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        return await context.ExperimentRuns
            .Where(r => r.SessionId == sessionId)
            .OrderBy(r => r.N)
            .ThenBy(r => r.M)
            .ThenBy(r => r.RunIndex)
            .ToListAsync(ct);
    }

    public async Task<List<ExperimentSessionEntity>> GetSessionsForAlgorithmAsync(
        string algorithmName,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        return await context.ExperimentSessions
            .Include(s => s.Algorithm)
            .Where(s => s.Algorithm.Name == algorithmName)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task DeleteSessionAsync(
        int sessionId,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        var session = await context.ExperimentSessions
            .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

        if (session is not null)
        {
            context.ExperimentSessions.Remove(session);
            await context.SaveChangesAsync(ct);
        }
    }
}
