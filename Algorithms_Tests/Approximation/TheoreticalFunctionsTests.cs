using Algorithms_programm.Approximation;
using Xunit;

namespace Algorithms_Tests.Approximation;

public class TheoreticalFunctionsTests
{
    [Theory]
    [InlineData(TheoreticalFunctionType.Constant, 1, 1)]
    [InlineData(TheoreticalFunctionType.Constant, 1000, 1)]
    [InlineData(TheoreticalFunctionType.Linear, 5, 5)]
    [InlineData(TheoreticalFunctionType.Quadratic, 5, 25)]
    [InlineData(TheoreticalFunctionType.Cubic, 5, 125)]
    [InlineData(TheoreticalFunctionType.Logarithmic, 8, 3)]     // log2(8) = 3
    [InlineData(TheoreticalFunctionType.Logarithmic, 1, 0)]     // log2(1) = 0
    [InlineData(TheoreticalFunctionType.LinearLogarithmic, 8, 24)] // 8 * log2(8) = 8*3
    public void Evaluate_OnKnownValues_ReturnsExpected(TheoreticalFunctionType type, double n, double expected)
    {
        var result = TheoreticalFunctions.Evaluate(type, n);

        Assert.Equal(expected, result, precision: 8);
    }

    [Fact]
    public void Evaluate_OnNLessThanOne_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TheoreticalFunctions.Evaluate(TheoreticalFunctionType.Linear, 0));
    }

    [Fact]
    public void AllTypes_ContainsExactlySixFunctionsFromMethodology()
    {
        Assert.Equal(6, TheoreticalFunctions.AllTypes.Count);
        Assert.Equal(TheoreticalFunctions.AllTypes.Distinct().Count(), TheoreticalFunctions.AllTypes.Count);
    }

    [Fact]
    public void GetDisplayName_ReturnsNonEmptyStringForEveryType()
    {
        foreach (var type in TheoreticalFunctions.AllTypes)
        {
            Assert.False(string.IsNullOrWhiteSpace(TheoreticalFunctions.GetDisplayName(type)));
        }
    }
}
