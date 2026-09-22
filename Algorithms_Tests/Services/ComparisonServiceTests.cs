using Algorithms_Tests.Database;
using Algorithms_programm.Caching;
using Algorithms_programm.Database.Repositories;
using Algorithms_programm.Services;
using Xunit;

namespace Algorithms_Tests.Services;

public class ComparisonServiceTests : IDisposable
{
    private readonly SqliteInMemoryContextFactory _factory = new();
    private readonly ExperimentOrchestrator _orchestrator;
    private readonly ComparisonService _comparisonService;

    public ComparisonServiceTests()
    {
        var repository = new EfExperimentRepository(_factory);
        var cache = new DbBackedCacheService(repository);
        _orchestrator = new ExperimentOrchestrator(new AlgorithmRegistry(), cache);
        _comparisonService = new ComparisonService(repository);
    }

    [Fact]
    public async Task GetSessionsAsync_AfterTwoDifferentRuns_ReturnsBothSessions()
    {
        await _orchestrator.RunExperimentAsync("Sum", nMax: 100, step: 50, runsPerPoint: 2, forceRecalculate: false);
        await _orchestrator.RunExperimentAsync("Sum", nMax: 500, step: 50, runsPerPoint: 2, forceRecalculate: false);

        var sessions = await _comparisonService.GetSessionsAsync("Sum");

        Assert.Equal(2, sessions.Count);
        Assert.All(sessions, s => Assert.Equal("Sum", s.AlgorithmName));
    }

    [Fact]
    public async Task GetSessionsAsync_ForDifferentAlgorithm_ReturnsEmpty()
    {
        await _orchestrator.RunExperimentAsync("Sum", nMax: 100, step: 50, runsPerPoint: 2, forceRecalculate: false);

        var sessions = await _comparisonService.GetSessionsAsync("Product");

        Assert.Empty(sessions);
    }

    [Fact]
    public async Task GetSessionPointsAsync_ReturnsSameNumberOfPointsAsOriginalRun()
    {
        var result = await _orchestrator.RunExperimentAsync(
            "Sum", nMax: 300, step: 100, runsPerPoint: 3, forceRecalculate: false);

        var points = await _comparisonService.GetSessionPointsAsync(result.SessionId);

        Assert.Equal(result.Points.Count, points.Count);
    }

    [Fact]
    public async Task GetSessionPointsAsync_OnUnknownSession_ReturnsEmpty()
    {
        var points = await _comparisonService.GetSessionPointsAsync(sessionId: 999_999);

        Assert.Empty(points);
    }

    public void Dispose() => _factory.Dispose();
}
