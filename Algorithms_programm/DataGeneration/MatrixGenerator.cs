namespace Algorithms_programm.DataGeneration;

/// <summary>
/// Генератор случайных матриц с неотрицательными элементами для Части II методички
/// (матричное умножение C = A*B).
/// </summary>
public sealed class MatrixGenerator
{
    private readonly Random _random;

    public MatrixGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>Генерирует матрицу rows x cols со случайными неотрицательными элементами.</summary>
    public double[,] Generate(int rows, int cols, double minValue = 0.0, double maxValue = 10.0)
    {
        if (rows < 0 || cols < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows), "Размеры матрицы не могут быть отрицательными.");
        }

        if (minValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue), "Элементы матрицы должны быть неотрицательными.");
        }

        var matrix = new double[rows, cols];
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                matrix[i, j] = minValue + _random.NextDouble() * (maxValue - minValue);
            }
        }

        return matrix;
    }
}
