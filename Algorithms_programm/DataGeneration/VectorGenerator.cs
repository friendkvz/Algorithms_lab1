namespace Algorithms_programm.DataGeneration;

/// <summary>
/// Генератор случайных векторов с неотрицательными элементами для Части I методички.
/// </summary>
public sealed class VectorGenerator
{
    private readonly Random _random;

    public VectorGenerator(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>
    /// Генерирует вектор длины n со случайными неотрицательными элементами из [minValue, maxValue].
    /// Значения по умолчанию (0.0, 100.0) достаточно "разнообразны", чтобы избегать
    /// вырожденных случаев (все элементы равны) при тестировании сортировок и полиномов,
    /// но не приводят к переполнению double даже при произведении большого числа элементов
    /// (для произведения при очень больших n это всё равно ограничение самого алгоритма 3,
    /// а не генератора — обрабатывается на уровне бенчмарка/UI предупреждением о possible overflow).
    /// </summary>
    public double[] Generate(int n, double minValue = 0.0, double maxValue = 100.0)
    {
        if (n < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "Размер вектора не может быть отрицательным.");
        }

        if (minValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minValue), "Элементы вектора должны быть неотрицательными.");
        }

        var vector = new double[n];
        for (var i = 0; i < n; i++)
        {
            vector[i] = minValue + _random.NextDouble() * (maxValue - minValue);
        }

        return vector;
    }
}
