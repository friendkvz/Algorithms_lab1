using Algorithms_programm.Services;
using Xunit;

namespace Algorithms_Tests.Services;

public class AlgorithmRegistryTests
{
    private readonly AlgorithmRegistry _registry = new();

    [Fact]
    public void All_ContainsExactlyThirteenAlgorithms()
    {
        // Векторные 1,2,3,4a,4b,5,6,7 (8 штук, т.к. 4 разбит на наивный/Горнер)
        // + 1 матричный (8) + 1 custom (HanSort, Часть III) + 3 степенных (10-12) = 13.
        Assert.Equal(14, _registry.All.Count);
    }

    [Theory]
    [InlineData("ConstantFunction")]
    [InlineData("Sum")]
    [InlineData("Product")]
    [InlineData("PolynomialNaive")]
    [InlineData("PolynomialHorner")]
    [InlineData("BubbleSort")]
    [InlineData("QuickSort")]
    [InlineData("BuiltInSort")]
    [InlineData("MatrixMultiplication")]
    [InlineData("HanSort")]
    [InlineData("IterativePower")]
    [InlineData("RecursivePower")]
    [InlineData("FastPower")]
    [InlineData("CycleSort")]
    public void Get_OnEveryKnownAlgorithmName_ReturnsRunner(string name)
    {
        var runner = _registry.Get(name);

        Assert.Equal(name, runner.Name);
    }

    [Fact]
    public void Get_OnUnknownName_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => _registry.Get("DoesNotExist"));
    }

    [Fact]
    public void MatrixMultiplication_IsFlaggedAsMatrixAndNotStepCounting()
    {
        var runner = _registry.Get("MatrixMultiplication");

        Assert.True(runner.IsMatrix);
        Assert.False(runner.IsStepCounting);
        Assert.Equal(AlgorithmCategory.Cubic, runner.Category);
    }

    [Theory]
    [InlineData("IterativePower")]
    [InlineData("RecursivePower")]
    [InlineData("FastPower")]
    public void PowerAlgorithms_AreFlaggedAsStepCountingAndNotMatrix(string name)
    {
        var runner = _registry.Get(name);

        Assert.True(runner.IsStepCounting);
        Assert.False(runner.IsMatrix);
        Assert.Equal(AlgorithmCategory.Power, runner.Category);
    }

    [Fact]
    public async Task TimedRunner_MeasureTimeRunsAsync_ReturnsRunsPerPointMeasurements()
    {
        var runner = _registry.Get("Sum");

        var results = await runner.MeasureTimeRunsAsync(n: 1000, runsPerPoint: 3, m: null);

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task TimedRunner_MeasureStepRunsAsync_Throws()
    {
        var runner = _registry.Get("Sum");

        await Assert.ThrowsAsync<NotSupportedException>(
            () => runner.MeasureStepRunsAsync(n: 100, runsPerPoint: 1));
    }

    [Fact]
    public async Task StepCountingRunner_MeasureTimeRunsAsync_Throws()
    {
        var runner = _registry.Get("FastPower");

        await Assert.ThrowsAsync<NotSupportedException>(
            () => runner.MeasureTimeRunsAsync(n: 100, runsPerPoint: 1, m: null));
    }

    [Fact]
    public async Task MatrixRunner_MeasureTimeRunsAsync_AcceptsSeparateNAndM()
    {
        var runner = _registry.Get("MatrixMultiplication");

        var results = await runner.MeasureTimeRunsAsync(n: 20, runsPerPoint: 2, m: 15);

        Assert.Equal(2, results.Count);
        Assert.All(results, t => Assert.True(t >= 0));
    }
}
