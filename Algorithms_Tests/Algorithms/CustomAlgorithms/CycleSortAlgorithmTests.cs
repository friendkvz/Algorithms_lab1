using Algorithms_programm.Algorithms.CustomAlgorithms;
using Xunit;

namespace Algorithms_Tests.Algorithms.CustomAlgorithms;

public class CycleSortAlgorithmTests
{
    [Fact]
    public void Execute_OnUnsortedArray_ProducesAscendingOrder()
    {
        var algorithm = new CycleSortAlgorithm();
        var input = new[] { 5.0, 3.2, 8.1, 1.0, 9.9, 2.4, 7.0, 4.5 };
        var expected = input.OrderBy(x => x).ToArray();

        var result = algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_OnEmptyArray_ReturnsEmptyArray()
    {
        var algorithm = new CycleSortAlgorithm();
        var result = algorithm.Execute(Array.Empty<double>());
        Assert.Empty(result);
    }

    [Fact]
    public void Execute_OnArrayWithDuplicates_SortsCorrectly()
    {
        var algorithm = new CycleSortAlgorithm();
        var input = new[] { 5.0, 1.0, 5.0, 1.0, 3.0, 3.0, 5.0, 0.0 };
        var expected = input.OrderBy(x => x).ToArray();

        var result = algorithm.Execute(input);

        Assert.Equal(expected, result);
    }
}