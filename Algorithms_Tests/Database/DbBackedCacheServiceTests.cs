using Algorithms_programm.Benchmarking;
using Algorithms_programm.Caching;
using Algorithms_programm.Database.Entities;
using Algorithms_programm.Database.Repositories;
using Xunit;

namespace Algorithms_Tests.Database;

public class DbBackedCacheServiceTests : IDisposable
{
    private readonly SqliteInMemoryContextFactory _factory = new();
    private readonly DbBackedCacheService _cache;

    public DbBackedCacheServiceTests()
    {
        var repository = new EfExperimentRepository(_factory);
        _cache = new DbBackedCacheService(repository);
    }

    [Fact]
    public async Task GetOrCreateSessionAsync_CalledTwiceWithSameConfig_ReturnsSameSessionId_NoForceRecalculate()
    {
        // Это и есть требование методички: "второй запуск с теми же параметрами не должен
        // пересчитывать, а брать из БД" — здесь проверяется, что вторая попытка получить
        // сессию для той же конфигурации возвращает ту же самую сессию (кэш-хит), а не создаёт новую.
        var config = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);

        var firstSessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate: false);
        var secondSessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate: false);

        Assert.Equal(firstSessionId, secondSessionId);
    }

    [Fact]
    public async Task GetOrCreateSessionAsync_WithForceRecalculate_CreatesNewSessionAndDropsOldData()
    {
        var config = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);

        var firstSessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate: false);
        await _cache.SaveRunsAsync(firstSessionId, new[]
        {
            new ExperimentRunEntity { N = 100, RunIndex = 1, ElapsedMilliseconds = 1.0, MeasuredAt = DateTime.UtcNow },
        });

        var secondSessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate: true);

        Assert.NotEqual(firstSessionId, secondSessionId);

        // Старые данные должны быть удалены вместе со старой сессией.
        var oldRuns = await _cache.GetCachedRunsAsync(firstSessionId, 100);
        Assert.Empty(oldRuns);
    }

    [Fact]
    public async Task GetOrCreateSessionAsync_DifferentConfigs_ProduceDifferentSessions()
    {
        var config1 = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);
        var config2 = new ExperimentConfig("Sum", NMax: 5000, Step: 10, RunsPerPoint: 5);

        var sessionId1 = await _cache.GetOrCreateSessionAsync(config1, forceRecalculate: false);
        var sessionId2 = await _cache.GetOrCreateSessionAsync(config2, forceRecalculate: false);

        Assert.NotEqual(sessionId1, sessionId2);
    }

    [Fact]
    public async Task IsPointCachedAsync_BeforeSaving_ReturnsFalse()
    {
        var config = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);
        var sessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate: false);

        var isCached = await _cache.IsPointCachedAsync(sessionId, n: 100, requiredRuns: 5);

        Assert.False(isCached);
    }

    [Fact]
    public async Task IsPointCachedAsync_AfterSavingEnoughRuns_ReturnsTrue()
    {
        var config = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);
        var sessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate: false);

        var runs = Enumerable.Range(1, 5).Select(i => new ExperimentRunEntity
        {
            N = 100, RunIndex = i, ElapsedMilliseconds = i, MeasuredAt = DateTime.UtcNow,
        });
        await _cache.SaveRunsAsync(sessionId, runs);

        var isCached = await _cache.IsPointCachedAsync(sessionId, n: 100, requiredRuns: 5);

        Assert.True(isCached);
    }

    [Fact]
    public async Task IsPointCachedAsync_WithPartialRuns_ReturnsFalse()
    {
        var config = new ExperimentConfig("Sum", NMax: 1000, Step: 10, RunsPerPoint: 5);
        var sessionId = await _cache.GetOrCreateSessionAsync(config, forceRecalculate: false);

        await _cache.SaveRunsAsync(sessionId, new[]
        {
            new ExperimentRunEntity { N = 100, RunIndex = 1, ElapsedMilliseconds = 1.0, MeasuredAt = DateTime.UtcNow },
            new ExperimentRunEntity { N = 100, RunIndex = 2, ElapsedMilliseconds = 1.0, MeasuredAt = DateTime.UtcNow },
        });

        // Сохранено только 2 из требуемых 5 запусков — точка ещё не считается закэшированной.
        var isCached = await _cache.IsPointCachedAsync(sessionId, n: 100, requiredRuns: 5);

        Assert.False(isCached);
    }

    public void Dispose() => _factory.Dispose();
}
