using Algorithms_programm.Approximation;
using Xunit;

namespace Algorithms_Tests.Approximation;

public class MseCalculatorTests
{
    [Fact]
    public void Calculate_OnPerfectFit_ReturnsZero()
    {
        var points = new[] { new DataPoint(1, 3), new DataPoint(2, 6), new DataPoint(3, 9) };

        var mse = MseCalculator.Calculate(points, TheoreticalFunctionType.Linear, c: 3.0);

        Assert.Equal(0.0, mse, precision: 8);
    }

    [Fact]
    public void Calculate_OnKnownErrors_MatchesManualCalculation()
    {
        // f(n) = n, C = 1 => T_approx = n. Точки: (1,2) err=1, (2,5) err=3, (3,3) err=0.
        // MSE = (1^2 + 3^2 + 0^2) / 3 = 10/3.
        var points = new[] { new DataPoint(1, 2), new DataPoint(2, 5), new DataPoint(3, 3) };

        var mse = MseCalculator.Calculate(points, TheoreticalFunctionType.Linear, c: 1.0);

        Assert.Equal(10.0 / 3.0, mse, precision: 8);
    }

    [Fact]
    public void Calculate_OnEmptyPoints_ReturnsZero()
    {
        var mse = MseCalculator.Calculate(Array.Empty<DataPoint>(), TheoreticalFunctionType.Linear, c: 1.0);

        Assert.Equal(0.0, mse, precision: 8);
    }

    [Fact]
    public void Calculate_WorseConstant_ProducesHigherMseThanFittedConstant()
    {
        var points = new[] { new DataPoint(1, 3), new DataPoint(2, 6), new DataPoint(5, 15), new DataPoint(10, 30) };
        var fittedC = LeastSquaresApproximator.FitConstant(points, TheoreticalFunctionType.Linear);

        var mseAtFitted = MseCalculator.Calculate(points, TheoreticalFunctionType.Linear, fittedC);
        var mseAtWorse = MseCalculator.Calculate(points, TheoreticalFunctionType.Linear, fittedC + 5);

        Assert.True(mseAtFitted <= mseAtWorse);
    }
}
