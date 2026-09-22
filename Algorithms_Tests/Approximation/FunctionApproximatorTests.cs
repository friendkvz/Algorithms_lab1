using Algorithms_programm.Approximation;
using Xunit;

namespace Algorithms_Tests.Approximation;

public class FunctionApproximatorTests
{
    [Fact]
    public void Approximate_OnExactQuadraticData_FitsQuadraticWithNearZeroMse()
    {
        var points = Enumerable.Range(1, 20).Select(n => new DataPoint(n, 2.0 * n * n)).ToArray();

        var result = FunctionApproximator.Approximate(points, TheoreticalFunctionType.Quadratic);

        Assert.Equal(2.0, result.C, precision: 6);
        Assert.True(result.Mse < 1e-6);
    }

    [Fact]
    public void ApproximateAll_ReturnsAllSixFunctionsSortedByAscendingMse()
    {
        var points = Enumerable.Range(1, 10).Select(n => new DataPoint(n, 5.0 * n)).ToArray();

        var results = FunctionApproximator.ApproximateAll(points);

        Assert.Equal(6, results.Count);
        for (var i = 1; i < results.Count; i++)
        {
            Assert.True(results[i - 1].Mse <= results[i].Mse);
        }
    }

    [Fact]
    public void FindBestFit_OnExactCubicData_SelectsCubicFunction()
    {
        var points = Enumerable.Range(1, 15).Select(n => new DataPoint(n, 0.5 * n * n * n)).ToArray();

        var best = FunctionApproximator.FindBestFit(points);

        Assert.Equal(TheoreticalFunctionType.Cubic, best.FunctionType);
        Assert.True(best.Mse < 1e-3);
    }

    [Fact]
    public void FindBestFit_OnExactLogData_SelectsLogarithmicFunction()
    {
        var points = Enumerable.Range(1, 20)
            .Select(n => new DataPoint(n, 4.0 * Math.Log2(n)))
            .Where(p => p.N > 1) // log2(1) = 0 - исключаем вырожденную точку из выборки
            .ToArray();

        var best = FunctionApproximator.FindBestFit(points);

        Assert.Equal(TheoreticalFunctionType.Logarithmic, best.FunctionType);
    }

    [Fact]
    public void Evaluate_OnFittedResult_ComputesCTimesF()
    {
        var result = new ApproximationResult(TheoreticalFunctionType.Linear, C: 2.5, Mse: 0);

        Assert.Equal(25.0, result.Evaluate(10), precision: 8);
    }
}
