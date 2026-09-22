using Algorithms_programm.Algorithms.MatrixOperations;
using Xunit;

namespace Algorithms_Tests.Algorithms.MatrixOperations;

public class MatrixMultiplicationAlgorithmTests
{
    [Fact]
    public void Execute_OnKnownSquareMatrices_ReturnsCorrectProduct()
    {
        // A = [[1,2],[3,4]], B = [[5,6],[7,8]]
        // A*B = [[19,22],[43,50]]
        var a = new double[,] { { 1, 2 }, { 3, 4 } };
        var b = new double[,] { { 5, 6 }, { 7, 8 } };
        var algorithm = new MatrixMultiplicationAlgorithm();

        var result = algorithm.Execute(new MatrixPair(a, b));

        Assert.Equal(19, result[0, 0], precision: 10);
        Assert.Equal(22, result[0, 1], precision: 10);
        Assert.Equal(43, result[1, 0], precision: 10);
        Assert.Equal(50, result[1, 1], precision: 10);
    }

    [Fact]
    public void Execute_OnRectangularMatrices_ReturnsCorrectDimensionsAndValues()
    {
        // A is 2x3, B is 3x2 => result is 2x2
        var a = new double[,] { { 1, 2, 3 }, { 4, 5, 6 } };
        var b = new double[,] { { 7, 8 }, { 9, 10 }, { 11, 12 } };
        var algorithm = new MatrixMultiplicationAlgorithm();

        var result = algorithm.Execute(new MatrixPair(a, b));

        Assert.Equal(2, result.GetLength(0));
        Assert.Equal(2, result.GetLength(1));
        // row0: [1*7+2*9+3*11, 1*8+2*10+3*12] = [58, 64]
        Assert.Equal(58, result[0, 0], precision: 10);
        Assert.Equal(64, result[0, 1], precision: 10);
        // row1: [4*7+5*9+6*11, 4*8+5*10+6*12] = [139, 154]
        Assert.Equal(139, result[1, 0], precision: 10);
        Assert.Equal(154, result[1, 1], precision: 10);
    }

    [Fact]
    public void Execute_OnIncompatibleDimensions_ThrowsArgumentException()
    {
        var a = new double[,] { { 1, 2 } }; // 1x2
        var b = new double[,] { { 1, 2 } }; // 1x2, incompatible (ColsA=2 != RowsB=1)
        var algorithm = new MatrixMultiplicationAlgorithm();

        Assert.Throws<ArgumentException>(() => algorithm.Execute(new MatrixPair(a, b)));
    }
}
