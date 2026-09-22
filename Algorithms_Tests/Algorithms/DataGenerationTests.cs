using Algorithms_programm.DataGeneration;
using Xunit;

namespace Algorithms_Tests.Algorithms;

public class DataGenerationTests
{
    [Fact]
    public void VectorGenerator_ProducesVectorOfCorrectLengthAndNonNegativeElements()
    {
        var generator = new VectorGenerator(seed: 42);

        var vector = generator.Generate(50, minValue: 0, maxValue: 10);

        Assert.Equal(50, vector.Length);
        Assert.All(vector, x => Assert.True(x is >= 0 and <= 10));
    }

    [Fact]
    public void VectorGenerator_WithSameSeed_ProducesReproducibleResults()
    {
        var vectorA = new VectorGenerator(seed: 123).Generate(20);
        var vectorB = new VectorGenerator(seed: 123).Generate(20);

        Assert.Equal(vectorA, vectorB);
    }

    [Fact]
    public void MatrixGenerator_ProducesMatrixOfCorrectDimensionsAndNonNegativeElements()
    {
        var generator = new MatrixGenerator(seed: 7);

        var matrix = generator.Generate(5, 8, minValue: 0, maxValue: 5);

        Assert.Equal(5, matrix.GetLength(0));
        Assert.Equal(8, matrix.GetLength(1));
        foreach (var value in matrix)
        {
            Assert.True(value is >= 0 and <= 5);
        }
    }

    [Fact]
    public void IntegerVectorGenerator_ProducesVectorOfCorrectLengthAndNonNegativeElements()
    {
        var generator = new IntegerVectorGenerator(seed: 99);

        var vector = generator.Generate(50, maxValue: 1000);

        Assert.Equal(50, vector.Length);
        Assert.All(vector, x => Assert.True(x is >= 0 and <= 1000));
    }

    [Fact]
    public void IntegerVectorGenerator_WithSameSeed_ProducesReproducibleResults()
    {
        var vectorA = new IntegerVectorGenerator(seed: 55).Generate(20);
        var vectorB = new IntegerVectorGenerator(seed: 55).Generate(20);

        Assert.Equal(vectorA, vectorB);
    }
}
