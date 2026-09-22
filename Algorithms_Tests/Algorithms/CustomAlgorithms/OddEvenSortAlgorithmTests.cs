using Algorithms_programm.Algorithms.CustomAlgorithms;
using Xunit;

namespace Algorithms_Tests.Algorithms.CustomAlgorithms;

public class OddEvenSortAlgorithmTests
{
    private readonly OddEvenSortAlgorithm _algorithm = new();

    [Fact]
    public void Execute_ShouldSortUnsortedArray()
    {
        var input = new[] { 5.0, 3.0, 8.0, 1.0, 9.0, 2.0 };
        var expected = new[] { 1.0, 2.0, 3.0, 5.0, 8.0, 9.0 };

        var result = _algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldSortArrayWithDuplicates()
    {
        var input = new[] { 4.0, 2.0, 4.0, 1.0, 2.0, 4.0 };
        var expected = new[] { 1.0, 2.0, 2.0, 4.0, 4.0, 4.0 };

        var result = _algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldHandleAlreadySortedArray()
    {
        var input = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };

        var result = _algorithm.Execute(input);

        Assert.Equal(input, result);
    }

    [Fact]
    public void Execute_ShouldHandleReverseSortedArray()
    {
        var input = new[] { 5.0, 4.0, 3.0, 2.0, 1.0 };
        var expected = new[] { 1.0, 2.0, 3.0, 4.0, 5.0 };

        var result = _algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldHandleEmptyArray()
    {
        var input = Array.Empty<double>();

        var result = _algorithm.Execute(input);

        Assert.Empty(result);
    }

    [Fact]
    public void Execute_ShouldHandleSingleElement()
    {
        var input = new[] { 42.0 };

        var result = _algorithm.Execute(input);

        Assert.Equal(new[] { 42.0 }, result);
    }

    [Fact]
    public void Execute_ShouldNotModifyOriginalArray()
    {
        var input = new[] { 5.0, 1.0, 4.0, 2.0, 3.0 };
        var original = (double[])input.Clone();

        _algorithm.Execute(input);

        Assert.Equal(original, input);
    }

    [Fact]
    public void Execute_ShouldHandleNegativeNumbers()
    {
        var input = new[] { -1.0, 5.0, -10.0, 3.0, 0.0 };
        var expected = new[] { -10.0, -1.0, 0.0, 3.0, 5.0 };

        var result = _algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Execute_ShouldHandleEqualElements()
    {
        var input = new[] { 7.0, 7.0, 7.0, 7.0 };

        var result = _algorithm.Execute(input);

        Assert.Equal(input, result);
    }
}


