namespace Algorithms_programm.Services;

/// <summary>
/// Категория теоретической сложности алгоритма — используется для дефолтных N_max
/// ("не хардкодить N_max... дефолтные значения задавать константами по категориям сложности").
/// </summary>
public enum AlgorithmCategory
{
    Constant,          // O(1)
    Linear,            // O(n)
    LinearLogarithmic, // O(n log n)
    Quadratic,         // O(n^2)
    Cubic,             // O(n^3) — матричное умножение
    Power,             // Часть IV: возведение в степень, измеряются шаги, не время
}
