using Algorithms_programm.Database.Entities;
using Algorithms_programm.Database.Repositories;
using Xunit;

namespace Algorithms_Tests.Database;

public class EfExperimentRepositoryTests : IDisposable
{
    private readonly SqliteInMemoryContextFactory _factory = new();
    private readonly EfExperimentRepository _repository;

    public EfExperimentRepositoryTests()
    {
        _repository = new EfExperimentRepository(_factory);
    }

    [Fact]
    public async Task GetOrCreateAlgorithmAsync_CalledTwiceWithSameName_ReturnsSameRow()
    {
        var first = await _repository.GetOrCreateAlgorithmAsync("Sum");
        var second = await _repository.GetOrCreateAlgorithmAsync("Sum");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task CreateSessionAsync_ThenFindByConfig_ReturnsCreatedSession()
    {
        var created = await _repository.CreateSessionAsync(
            algorithmName: "QuickSort", nMax: 10000, step: 100, runsPerPoint: 5, configHash: "hash-1");

        var found = await _repository.FindSessionByConfigAsync("QuickSort", "hash-1");

        Assert.NotNull(found);
        Assert.Equal(created.Id, found!.Id);
        Assert.Equal(10000, found.NMax);
    }

    [Fact]
    public async Task FindSessionByConfigAsync_OnDifferentHash_ReturnsNull()
    {
        await _repository.CreateSessionAsync(
            algorithmName: "QuickSort", nMax: 10000, step: 100, runsPerPoint: 5, configHash: "hash-1");

        var found = await _repository.FindSessionByConfigAsync("QuickSort", "hash-2");

        Assert.Null(found);
    }

    [Fact]
    public async Task AddRunsAsync_ThenGetRuns_ReturnsAllRunsForThePoint()
    {
        var session = await _repository.CreateSessionAsync(
            algorithmName: "Sum", nMax: 1000, step: 10, runsPerPoint: 5, configHash: "hash-sum");

        var runs = Enumerable.Range(1, 5).Select(i => new ExperimentRunEntity
        {
            SessionId = session.Id,
            N = 100,
            RunIndex = i,
            ElapsedMilliseconds = i * 1.5,
            MeasuredAt = DateTime.UtcNow,
        });

        await _repository.AddRunsAsync(runs);

        var stored = await _repository.GetRunsAsync(session.Id, 100);

        Assert.Equal(5, stored.Count);
    }

    [Fact]
    public async Task GetRunsAsync_ForUnmeasuredPoint_ReturnsEmpty()
    {
        var session = await _repository.CreateSessionAsync(
            algorithmName: "Sum", nMax: 1000, step: 10, runsPerPoint: 5, configHash: "hash-sum-2");

        var stored = await _repository.GetRunsAsync(session.Id, 999);

        Assert.Empty(stored);
    }

    [Fact]
    public async Task DeleteSessionAsync_RemovesSessionAndItsRuns()
    {
        var session = await _repository.CreateSessionAsync(
            algorithmName: "Sum", nMax: 1000, step: 10, runsPerPoint: 5, configHash: "hash-del");

        await _repository.AddRunsAsync(new[]
        {
            new ExperimentRunEntity
            {
                SessionId = session.Id, N = 100, RunIndex = 1,
                ElapsedMilliseconds = 1.0, MeasuredAt = DateTime.UtcNow,
            },
        });

        await _repository.DeleteSessionAsync(session.Id);

        var found = await _repository.FindSessionByConfigAsync("Sum", "hash-del");
        var runs = await _repository.GetRunsAsync(session.Id, 100);

        Assert.Null(found);
        Assert.Empty(runs);
    }

    [Fact]
    public async Task GetSessionsForAlgorithmAsync_ReturnsOnlySessionsOfThatAlgorithm()
    {
        await _repository.CreateSessionAsync("Sum", 1000, 10, 5, "hash-a");
        await _repository.CreateSessionAsync("Sum", 2000, 20, 5, "hash-b");
        await _repository.CreateSessionAsync("Product", 1000, 10, 5, "hash-c");

        var sumSessions = await _repository.GetSessionsForAlgorithmAsync("Sum");

        Assert.Equal(2, sumSessions.Count);
        Assert.All(sumSessions, s => Assert.Equal("Sum", s.Algorithm.Name));
    }

    public void Dispose() => _factory.Dispose();
}
