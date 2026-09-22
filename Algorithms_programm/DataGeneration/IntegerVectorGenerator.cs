namespace Algorithms_programm.DataGeneration;

/// <summary>
/// Генератор случайных векторов неотрицательных целых чисел — нужен для custom-алгоритма
/// Части III (HanSortAlgorithm), который работает с int[], а не с double[] как остальные
/// векторные операции.
/// </summary>
public sealed class IntegerVectorGenerator
{
    private readonly Random _random;

    public IntegerVectorGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>Генерирует вектор длины n со случайными целыми из [0, maxValue].</summary>
    public int[] Generate(int n, int maxValue = 1_000_000)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "Размер вектора не может быть отрицательным.");
        }

        if (maxValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxValue), "Значения должны быть неотрицательными.");
        }

        var vector = new int[n];
        for (var i = 0; i < n; i++)
        {
            vector[i] = _random.Next(0, maxValue + 1);
        }

        return vector;
    }
}
