using Algorithms_programm.Algorithms.VectorOperations;
using Xunit;

namespace Algorithms_Tests.Algorithms.VectorOperations;

public class BasicVectorAlgorithmsTests
{
    [Fact]
    public void ConstantFunction_AlwaysReturnsOne()
    {
        var algorithm = new ConstantFunctionAlgorithm();

        Assert.Equal(1, algorithm.Execute(Array.Empty<double>()));
        Assert.Equal(1, algorithm.Execute(new double[] { 1, 2, 3 }));
    }

    [Fact]
    public void Sum_OnKnownVector_ReturnsCorrectSum()
    {
        var algorithm = new SumAlgorithm();
        var vector = new double[] { 1, 2, 3, 4, 5 };

        var result = algorithm.Execute(vector);

        Assert.Equal(15, result, precision: 10);
    }

    [Fact]
    public void Sum_OnEmptyVector_ReturnsZero()
    {
        var algorithm = new SumAlgorithm();

        Assert.Equal(0, algorithm.Execute(Array.Empty<double>()), precision: 10);
    }

    [Fact]
    public void Product_OnKnownVector_ReturnsCorrectProduct()
    {
        var algorithm = new ProductAlgorithm();
        var vector = new double[] { 1, 2, 3, 4 };

        var result = algorithm.Execute(vector);

        Assert.Equal(24, result, precision: 10);
    }

    [Fact]
    public void Product_OnEmptyVector_ReturnsOne()
    {
        var algorithm = new ProductAlgorithm();

        Assert.Equal(1, algorithm.Execute(Array.Empty<double>()), precision: 10);
    }

    [Fact]
    public void PolynomialNaiveAndHorner_AgreeOnSameCoefficients()
    {
        var naive = new PolynomialNaiveAlgorithm();
        var horner = new PolynomialHornerAlgorithm();
        var coefficients = new double[] { 2, -3, 0.5, 1.25, 4 };

        var naiveResult = naive.Execute(coefficients);
        var hornerResult = horner.Execute(coefficients);

        Assert.Equal(naiveResult, hornerResult, precision: 8);
    }

    [Fact]
    public void PolynomialHorner_OnKnownCoefficients_MatchesManualCalculation()
    {
        // P(x) = 1 + 2x, x = 1.5 => 1 + 3 = 4
        var horner = new PolynomialHornerAlgorithm();
        var coefficients = new double[] { 1, 2 };

        var result = horner.Execute(coefficients);

        Assert.Equal(4.0, result, precision: 10);
    }

    [Fact]
    public void PolynomialNaive_OnKnownCoefficients_MatchesManualCalculation()
    {
        // P(x) = 1 + 2x, x = 1.5 => 1 + 3 = 4
        var naive = new PolynomialNaiveAlgorithm();
        var coefficients = new double[] { 1, 2 };

        var result = naive.Execute(coefficients);

        Assert.Equal(4.0, result, precision: 10);
    }
}
