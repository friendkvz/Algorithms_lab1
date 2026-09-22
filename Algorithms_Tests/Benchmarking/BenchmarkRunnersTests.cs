using Algorithms_programm.Algorithms.PowerAlgorithms;
using Algorithms_programm.Algorithms.VectorOperations;
using Algorithms_programm.Benchmarking;
using Algorithms_programm.DataGeneration;
using Xunit;

namespace Algorithms_Tests.Benchmarking;

public class BenchmarkRunnersTests
{
    [Fact]
    public void TimeBenchmarkRunner_ReturnsOneMeasurementPerRun()
    {
        var runner = new TimeBenchmarkRunner();
        var algorithm = new SumAlgorithm();
        var generator = new VectorGenerator(seed: 1);

        var results = runner.MeasureMilliseconds(algorithm, () => generator.Generate(1000), runsPerPoint: 5);

        Assert.Equal(5, results.Count);
        Assert.All(results, t => Assert.True(t >= 0));
    }

    [Fact]
    public void TimeBenchmarkRunner_OnInvalidRunsCount_Throws()
    {
        var runner = new TimeBenchmarkRunner();
        var algorithm = new SumAlgorithm();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => runner.MeasureMilliseconds(algorithm, () => Array.Empty<double>(), runsPerPoint: 0));
    }

    [Fact]
    public void TimeBenchmarkRunner_Average_OnEmptyList_ReturnsZero()
    {
        Assert.Equal(0, TimeBenchmarkRunner.Average(Array.Empty<double>()));
    }

    [Fact]
    public async Task TimeBenchmarkRunner_MeasureMillisecondsAsync_MatchesSyncResultCount()
    {
        var runner = new TimeBenchmarkRunner();
        var algorithm = new SumAlgorithm();
        var generator = new VectorGenerator(seed: 2);

        var results = await runner.MeasureMillisecondsAsync(algorithm, () => generator.Generate(500), runsPerPoint: 3);

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void StepCountBenchmarkRunner_ForFixedN_StepsAreConstantAcrossRuns()
    {
        // Число шагов зависит только от n, а не от случайного x — все runsPerPoint замеров
        // для одного и того же n должны дать одинаковое число шагов.
        var runner = new StepCountBenchmarkRunner();
        var algorithm = new FastPowerAlgorithm();
        var random = new Random(3);

        var results = runner.MeasureSteps(
            algorithm,
            () => new PowerInput(1.0 + random.NextDouble(), 777),
            runsPerPoint: 5);

        Assert.Equal(5, results.Count);
        Assert.True(results.Distinct().Count() == 1, "Число шагов должно быть одинаковым для фиксированного n.");
    }

    [Fact]
    public void StepCountBenchmarkRunner_Average_OnEmptyList_ReturnsZero()
    {
        Assert.Equal(0, StepCountBenchmarkRunner.Average(Array.Empty<long>()));
    }
}
