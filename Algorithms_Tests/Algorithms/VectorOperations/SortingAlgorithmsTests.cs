using Algorithms_programm.Algorithms.Abstractions;
using Algorithms_programm.Algorithms.VectorOperations;
using Xunit;

namespace Algorithms_Tests.Algorithms.VectorOperations;

public class SortingAlgorithmsTests
{
    public static IEnumerable<object[]> SortingAlgorithms()
    {
        yield return new object[] { new BubbleSortAlgorithm() };
        yield return new object[] { new QuickSortAlgorithm() };
        yield return new object[] { new BuiltInSortAlgorithm() };
    }

    [Theory]
    [MemberData(nameof(SortingAlgorithms))]
    public void Sort_OnUnsortedVector_ProducesAscendingOrder(ITimedAlgorithm<double[], double[]> algorithm)
    {
        var input = new double[] { 5, 3, 8, 1, 9, 2, 7, 4, 6, 0 };
        var expected = input.OrderBy(x => x).ToArray();

        var result = algorithm.Execute(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(SortingAlgorithms))]
    public void Sort_DoesNotMutateInputArray(ITimedAlgorithm<double[], double[]> algorithm)
    {
        var input = new double[] { 5, 3, 8, 1, 9 };
        var inputCopy = (double[])input.Clone();

        algorithm.Execute(input);

        Assert.Equal(inputCopy, input);
    }

    [Theory]
    [MemberData(nameof(SortingAlgorithms))]
    public void Sort_OnEmptyAndSingleElementVectors_DoesNotThrow(ITimedAlgorithm<double[], double[]> algorithm)
    {
        Assert.Empty(algorithm.Execute(Array.Empty<double>()));
        Assert.Equal(new double[] { 42 }, algorithm.Execute(new double[] { 42 }));
    }

    [Theory]
    [MemberData(nameof(SortingAlgorithms))]
    public void Sort_OnAlreadySortedVector_KeepsOrder(ITimedAlgorithm<double[], double[]> algorithm)
    {
        var input = new double[] { 1, 2, 3, 4, 5 };

        var result = algorithm.Execute(input);

        Assert.Equal(input, result);
    }

    [Theory]
    [MemberData(nameof(SortingAlgorithms))]
    public void Sort_OnVectorWithDuplicates_SortsCorrectly(ITimedAlgorithm<double[], double[]> algorithm)
    {
        var input = new double[] { 3, 1, 3, 1, 2, 2, 3 };
        var expected = input.OrderBy(x => x).ToArray();

        var result = algorithm.Execute(input);

        Assert.Equal(expected, result);
    }
}
