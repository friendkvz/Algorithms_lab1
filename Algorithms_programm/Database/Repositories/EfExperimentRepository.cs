using Algorithms_programm.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Algorithms_programm.Database.Repositories;

/// <summary>
/// Реализация поверх EF Core. Использует IDbContextFactory&lt;AppDbContext&gt;, а не единый
/// внедрённый AppDbContext: бенчмарки выполняются асинхронно и потенциально параллельно
/// (несколько точек n), а DbContext не потокобезопасен и не рассчитан на переиспользование
/// между конкурентными операциями. Каждый метод открывает короткоживущий контекст.
/// </summary>
public sealed class EfExperimentRepository : IExperimentRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public EfExperimentRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<AlgorithmEntity> GetOrCreateAlgorithmAsync(string algorithmName, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        var existing = await context.Algorithms.FirstOrDefaultAsync(a => a.Name == algorithmName, ct);
        if (existing is not null)
        {
            return existing;
        }

        var created = new AlgorithmEntity { Name = algorithmName };
        context.Algorithms.Add(created);
        await context.SaveChangesAsync(ct);
        return created;
    }

    public async Task<ExperimentSessionEntity?> FindSessionByConfigAsync(
        string algorithmName, string configHash, CancellationToken ct = default)
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

        var algorithm = await context.Algorithms.FirstOrDefaultAsync(a => a.Name == algorithmName, ct);
        if (algorithm is null)
        {
            algorithm = new AlgorithmEntity { Name = algorithmName };
            context.Algorithms.Add(algorithm);
        }

        var session = new ExperimentSessionEntity
        {
            Algorithm = algorithm,
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

    public async Task AddRunsAsync(IEnumerable<ExperimentRunEntity> runs, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);
        context.ExperimentRuns.AddRange(runs);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<ExperimentRunEntity>> GetRunsAsync(
        int sessionId, int n, int? m = null, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        return await context.ExperimentRuns
            .Where(r => r.SessionId == sessionId && r.N == n && r.M == m)
            .ToListAsync(ct);
    }

    public async Task<List<ExperimentRunEntity>> GetAllRunsForSessionAsync(int sessionId, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        return await context.ExperimentRuns
            .Where(r => r.SessionId == sessionId)
            .OrderBy(r => r.N).ThenBy(r => r.M).ThenBy(r => r.RunIndex)
            .ToListAsync(ct);
    }

    public async Task<List<ExperimentSessionEntity>> GetSessionsForAlgorithmAsync(
        string algorithmName, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        return await context.ExperimentSessions
            .Include(s => s.Algorithm)
            .Where(s => s.Algorithm.Name == algorithmName)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task DeleteSessionAsync(int sessionId, CancellationToken ct = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(ct);

        var session = await context.ExperimentSessions.FirstOrDefaultAsync(s => s.Id == sessionId, ct);
        if (session is not null)
        {
            context.ExperimentSessions.Remove(session); // каскад удалит связанные ExperimentRunEntity
            await context.SaveChangesAsync(ct);
        }
    }
}
