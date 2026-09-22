using Algorithms_programm.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Algorithms_programm.Database.Repositories;

/// <summary>
/// Реализация репозитория поверх EF Core. Каждый вызов использует отдельный короткоживущий
/// DbContext, поэтому репозиторий безопасен для асинхронного запуска экспериментов.
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

        // Сначала гарантированно сохраняем AlgorithmEntity и получаем его реальный PK.
        // Это важно для SQLite in-memory: перед добавлением ExperimentRunEntity внешний
        // ключ SessionId должен ссылаться на уже сохранённую цепочку Algorithm -> Session.
        var algorithm = await context.Algorithms
            .SingleOrDefaultAsync(a => a.Name == algorithmName, ct);

        if (algorithm is null)
        {
            algorithm = new AlgorithmEntity { Name = algorithmName };
            context.Algorithms.Add(algorithm);
            await context.SaveChangesAsync(ct);
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
        return session;
    }

    public async Task AddRunsAsync(
        IEnumerable<ExperimentRunEntity> runs,
        CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        var runList = runs.ToList();

        if (runList.Count == 0)
        {
            return;
        }

        var sessionIds = runList.Select(r => r.SessionId).Distinct().ToArray();
        var existingSessionIds = await context.ExperimentSessions
            .Where(s => sessionIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync(ct);

        var missingSessionId = sessionIds.FirstOrDefault(id => !existingSessionIds.Contains(id));
        if (missingSessionId != 0)
        {
            throw new InvalidOperationException(
                $"Невозможно сохранить замеры: сессия эксперимента {missingSessionId} не найдена.");
        }

        context.ExperimentRuns.AddRange(runList);
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
