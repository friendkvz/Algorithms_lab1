using Algorithms_programm.Algorithms.Abstractions;
using Algorithms_programm.Algorithms.PowerAlgorithms;
using Xunit;

namespace Algorithms_Tests.Algorithms.PowerAlgorithms;

public class PowerAlgorithmsTests
{
    public static IEnumerable<object[]> PowerAlgorithms()
    {
        yield return new object[] { new IterativePowerAlgorithm() };
        yield return new object[] { new RecursivePowerAlgorithm() };
        yield return new object[] { new FastPowerAlgorithm() };
    }

    [Theory]
    [MemberData(nameof(PowerAlgorithms))]
    public void Execute_OnSimpleN_ReturnsCorrectResult(IStepCountingAlgorithm<PowerInput, double> algorithm)
    {
        // 2^5 = 32
        var (result, _) = algorithm.ExecuteCountingSteps(new PowerInput(2.0, 5));
        Assert.Equal(32.0, result, precision: 8);

        // 3^0 = 1
        var (resultZero, stepsZero) = algorithm.ExecuteCountingSteps(new PowerInput(3.0, 0));
        Assert.Equal(1.0, resultZero, precision: 8);
        Assert.Equal(0, stepsZero);

        // 1.5^1 = 1.5
        var (resultOne, _) = algorithm.ExecuteCountingSteps(new PowerInput(1.5, 1));
        Assert.Equal(1.5, resultOne, precision: 8);
    }

    [Theory]
    [MemberData(nameof(PowerAlgorithms))]
    public void Execute_OnLargerN_MatchesMathPow(IStepCountingAlgorithm<PowerInput, double> algorithm)
    {
        var (result, _) = algorithm.ExecuteCountingSteps(new PowerInput(1.2, 20));
        Assert.Equal(Math.Pow(1.2, 20), result, precision: 6);
    }

    [Theory]
    [MemberData(nameof(PowerAlgorithms))]
    public void Execute_OnNegativeN_ThrowsArgumentException(IStepCountingAlgorithm<PowerInput, double> algorithm)
    {
        Assert.Throws<ArgumentException>(() => algorithm.ExecuteCountingSteps(new PowerInput(2.0, -1)));
    }

    [Fact]
    public void IterativePower_StepCount_IsNMinusOne()
    {
        var algorithm = new IterativePowerAlgorithm();

        var (_, steps) = algorithm.ExecuteCountingSteps(new PowerInput(1.1, 100));

        Assert.Equal(99, steps);
    }

    [Fact]
    public void RecursivePower_StepCount_IsN()
    {
        var algorithm = new RecursivePowerAlgorithm();

        var (_, steps) = algorithm.ExecuteCountingSteps(new PowerInput(1.1, 100));

        Assert.Equal(100, steps);
    }

    [Fact]
    public void FastPower_StepCount_IsSignificantlySmallerThanLinearForLargeN()
    {
        var fast = new FastPowerAlgorithm();
        var iterative = new IterativePowerAlgorithm();

        var (_, fastSteps) = fast.ExecuteCountingSteps(new PowerInput(1.001, 1000));
        var (_, iterativeSteps) = iterative.ExecuteCountingSteps(new PowerInput(1.001, 1000));

        // log2(1000) ~= 10, с округлениями бинарного возведения даём запас до 2*log2(n)+2
        Assert.True(fastSteps <= 2 * Math.Log2(1000) + 2);
        Assert.True(fastSteps < iterativeSteps);
    }
}
