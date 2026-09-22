using Algorithms_programm.Algorithms.CustomAlgorithms;
using Xunit;

namespace Algorithms_Tests.Algorithms.CustomAlgorithms;

public class HanSortAlgorithmTests
{
    [Fact]
    public void Execute_OnUnsortedArray_ProducesAscendingOrder()
    {
        var algorithm = new HanSortAlgorithm();
        var input = new[] { 500, 3, 65536, 42, 0, 999999, 16, 65535, 65537 };
        var expected = input.OrderBy(x => x).ToArray();

        var result = algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_OnEmptyArray_ReturnsEmptyArray()
    {
        var algorithm = new HanSortAlgorithm();

        var result = algorithm.Execute(Array.Empty<int>());

        Assert.Empty(result);
    }

    [Fact]
    public void Execute_OnSingleElement_ReturnsSameElement()
    {
        var algorithm = new HanSortAlgorithm();

        var result = algorithm.Execute(new[] { 42 });

        Assert.Equal(new[] { 42 }, result);
    }

    [Fact]
    public void Execute_OnArrayWithDuplicates_SortsCorrectly()
    {
        var algorithm = new HanSortAlgorithm();
        var input = new[] { 5, 1, 5, 1, 3, 3, 5, 0 };
        var expected = input.OrderBy(x => x).ToArray();

        var result = algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_OnMaxIntBoundaryValues_SortsCorrectly()
    {
        var algorithm = new HanSortAlgorithm();
        var input = new[] { int.MaxValue, 0, int.MaxValue / 2, 1, int.MaxValue - 1 };
        var expected = input.OrderBy(x => x).ToArray();

        var result = algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_OnNegativeValue_ThrowsArgumentException()
    {
        var algorithm = new HanSortAlgorithm();

        Assert.Throws<ArgumentException>(() => algorithm.Execute(new[] { 1, -2, 3 }));
    }
}
