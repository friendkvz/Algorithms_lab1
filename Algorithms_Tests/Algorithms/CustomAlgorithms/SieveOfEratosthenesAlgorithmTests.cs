using Algorithms_programm.Algorithms.CustomAlgorithms;
using Xunit;

namespace Algorithms_Tests.Algorithms.CustomAlgorithms;

public class SieveOfEratosthenesAlgorithmTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(10, 4)]   // 2,3,5,7
    [InlineData(20, 8)]   // 2,3,5,7,11,13,17,19
    public void Execute_OnKnownN_ReturnsCorrectPrimeCount(int n, int expectedCount)
    {
        var algorithm = new SieveOfEratosthenesAlgorithm();

        var result = algorithm.Execute(n);

        Assert.Equal(expectedCount, result);
    }
}
