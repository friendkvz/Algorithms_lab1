using Algorithms_programm.Approximation;
using Xunit;

namespace Algorithms_Tests.Approximation;

public class LeastSquaresApproximatorTests
{
    [Fact]
    public void FitConstant_OnExactLinearData_RecoversTrueConstant()
    {
        // T(n) = 3*n ровно, без шума — МНК должен вернуть C = 3 для f(n) = n.
        var points = new[]
        {
            new DataPoint(1, 3), new DataPoint(2, 6), new DataPoint(3, 9),
            new DataPoint(10, 30), new DataPoint(100, 300),
        };

        var c = LeastSquaresApproximator.FitConstant(points, TheoreticalFunctionType.Linear);

        Assert.Equal(3.0, c, precision: 8);
    }

    [Fact]
    public void FitConstant_OnExactQuadraticData_RecoversTrueConstant()
    {
        // T(n) = 2*n^2 ровно — МНК должен вернуть C = 2 для f(n) = n^2.
        var points = new[]
        {
            new DataPoint(1, 2), new DataPoint(2, 8), new DataPoint(4, 32), new DataPoint(10, 200),
        };

        var c = LeastSquaresApproximator.FitConstant(points, TheoreticalFunctionType.Quadratic);

        Assert.Equal(2.0, c, precision: 8);
    }

    [Fact]
    public void FitConstant_OnEmptyPoints_Throws()
    {
        Assert.Throws<ArgumentException>(
            () => LeastSquaresApproximator.FitConstant(Array.Empty<DataPoint>(), TheoreticalFunctionType.Linear));
    }

    [Fact]
    public void FitConstant_OnDegenerateDenominator_ReturnsZeroInsteadOfThrowing()
    {
        // Logarithmic при n=1 даёт f(n)=0 для всех точек => знаменатель суммы f(n)^2 равен 0.
        var points = new[] { new DataPoint(1, 5), new DataPoint(1, 7) };

        var c = LeastSquaresApproximator.FitConstant(points, TheoreticalFunctionType.Logarithmic);

        Assert.Equal(0.0, c, precision: 8);
    }
}
