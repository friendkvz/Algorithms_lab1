using Algorithms_Tests.Database;
using Algorithms_programm.Caching;
using Algorithms_programm.Database.Repositories;
using Algorithms_programm.Models;
using Algorithms_programm.Services;
using Xunit;

namespace Algorithms_Tests.Services;

public class ExperimentOrchestratorTests : IDisposable
{
    private readonly SqliteInMemoryContextFactory _factory = new();
    private readonly EfExperimentRepository _repository;
    private readonly ExperimentOrchestrator _orchestrator;

    public ExperimentOrchestratorTests()
    {
        _repository = new EfExperimentRepository(_factory);
        var cache = new DbBackedCacheService(_repository);
        _orchestrator = new ExperimentOrchestrator(new AlgorithmRegistry(), cache);
    }

    [Fact]
    public async Task RunExperimentAsync_OnSimpleAlgorithm_ProducesOnePointPerNAndSixApproximations()
    {
        // NMax=500, Step=100 => n = 1, 101, 201, 301, 401, 500 (500 добавляется отдельно) = 6 точек.
        var result = await _orchestrator.RunExperimentAsync(
            "Sum", nMax: 500, step: 100, runsPerPoint: 3, forceRecalculate: false);

        Assert.Equal(6, result.Points.Count);
        Assert.Equal(6, result.Approximations.Count);
        Assert.All(result.Points, p => Assert.Equal(3, p.RunCount));
        Assert.NotNull(result.BestFit);
    }

    [Fact]
    public async Task RunExperimentAsync_CalledTwiceWithSameConfig_ReusesSameSessionAndDoesNotDuplicateRuns()
    {
        var first = await _orchestrator.RunExperimentAsync(
            "Sum", nMax: 200, step: 100, runsPerPoint: 3, forceRecalculate: false);

        var second = await _orchestrator.RunExperimentAsync(
            "Sum", nMax: 200, step: 100, runsPerPoint: 3, forceRecalculate: false);

        Assert.Equal(first.SessionId, second.SessionId);

        // Требование методички: повторный запуск с теми же параметрами не пересчитывает —
        // в БД должно остаться ровно RunsPerPoint=3 запуска на точку n=1, а не 6.
        var runsForFirstPoint = await _repository.GetRunsAsync(first.SessionId, n: 1);
        Assert.Equal(3, runsForFirstPoint.Count);
    }

    [Fact]
    public async Task RunExperimentAsync_WithForceRecalculate_CreatesNewSession()
    {
        var first = await _orchestrator.RunExperimentAsync(
            "Sum", nMax: 200, step: 100, runsPerPoint: 3, forceRecalculate: false);

        var second = await _orchestrator.RunExperimentAsync(
            "Sum", nMax: 200, step: 100, runsPerPoint: 3, forceRecalculate: true);

        Assert.NotEqual(first.SessionId, second.SessionId);
    }

    [Fact]
    public async Task RunExperimentAsync_OnMatrixAlgorithm_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orchestrator.RunExperimentAsync(
                "MatrixMultiplication", nMax: 100, step: 10, runsPerPoint: 1, forceRecalculate: false));
    }

    [Fact]
    public async Task RunExperimentAsync_ReportsProgressUpToOne()
    {
        var reported = new List<double>();
        var progress = new Progress<double>(reported.Add);

        await _orchestrator.RunExperimentAsync(
            "Sum", nMax: 300, step: 100, runsPerPoint: 2, forceRecalculate: false, progress);

        // Progress<T> может репортить асинхронно, поэтому просто проверяем, что финальное
        // значение действительно достигло 1.0, а не что каждый отчёт пришёл синхронно по счёту.
        await Task.Delay(50); // даём Progress<T> шанс доставить последний callback
        Assert.Contains(reported, v => Math.Abs(v - 1.0) < 1e-9);
    }

    [Fact]
    public async Task RunExperimentAsync_OnStepCountingAlgorithm_UsesStepCountKind()
    {
        var result = await _orchestrator.RunExperimentAsync(
            "FastPower", nMax: 100, step: 50, runsPerPoint: 2, forceRecalculate: false);

        Assert.All(result.Points, p => Assert.Equal(MeasureKind.StepCount, p.Kind));
    }

    [Fact]
    public async Task RunMatrixExperimentAsync_ProducesGridOfNTimesMPoints()
    {
        // n = 1, 51, 100 (3 значения), m = 1, 26, 50 (3 значения) => 9 точек.
        var result = await _orchestrator.RunMatrixExperimentAsync(
            "MatrixMultiplication", nMax: 100, step: 50, mMax: 50, mStep: 25, runsPerPoint: 1, forceRecalculate: false);

        Assert.Equal(9, result.Points.Count);
        Assert.All(result.Points, p => Assert.NotNull(p.M));
    }

    public void Dispose() => _factory.Dispose();
}
