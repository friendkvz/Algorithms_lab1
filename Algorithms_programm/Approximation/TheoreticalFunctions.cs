namespace Algorithms_programm.Approximation;

/// <summary>
/// Набор теоретических функций f(n) из методички, среди которых подбирается аппроксимация
/// T_approx(n) = C * f(n). Домен всех функций — n >= 1 (методичка перебирает n от 1 до N_max),
/// поэтому Math.Log2 всегда получает аргумент >= 1 и не уходит в -Infinity/NaN.
/// </summary>
public enum TheoreticalFunctionType
{
    /// <summary>f(n) = 1 — константа, соответствует O(1).</summary>
    Constant,

    /// <summary>f(n) = n — соответствует O(n).</summary>
    Linear,

    /// <summary>f(n) = n * log2(n) — соответствует O(n log n).</summary>
    LinearLogarithmic,

    /// <summary>f(n) = n^2 — соответствует O(n^2).</summary>
    Quadratic,

    /// <summary>f(n) = n^3 — соответствует O(n^3).</summary>
    Cubic,

    /// <summary>f(n) = log2(n) — соответствует O(log n).</summary>
    Logarithmic,
}

public static class TheoreticalFunctions
{
    private static readonly TheoreticalFunctionType[] All =
    {
        TheoreticalFunctionType.Constant,
        TheoreticalFunctionType.Linear,
        TheoreticalFunctionType.LinearLogarithmic,
        TheoreticalFunctionType.Quadratic,
        TheoreticalFunctionType.Cubic,
        TheoreticalFunctionType.Logarithmic,
    };

    public static IReadOnlyList<TheoreticalFunctionType> AllTypes => All;

    /// <summary>Вычисляет f(n) для заданного типа теоретической функции. Требует n >= 1.</summary>
    public static double Evaluate(TheoreticalFunctionType type, double n)
    {
        if (n < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(n), "Теоретические функции определены для n >= 1.");
        }

        return type switch
        {
            TheoreticalFunctionType.Constant => 1.0,
            TheoreticalFunctionType.Linear => n,
            TheoreticalFunctionType.LinearLogarithmic => n * Math.Log2(n),
            TheoreticalFunctionType.Quadratic => n * n,
            TheoreticalFunctionType.Cubic => n * n * n,
            TheoreticalFunctionType.Logarithmic => Math.Log2(n),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Неизвестный тип теоретической функции."),
        };
    }

    /// <summary>Человекочитаемое обозначение f(n), для подписей на графике/в GUI.</summary>
    public static string GetDisplayName(TheoreticalFunctionType type) => type switch
    {
        TheoreticalFunctionType.Constant => "1",
        TheoreticalFunctionType.Linear => "n",
        TheoreticalFunctionType.LinearLogarithmic => "n·log(n)",
        TheoreticalFunctionType.Quadratic => "n²",
        TheoreticalFunctionType.Cubic => "n³",
        TheoreticalFunctionType.Logarithmic => "log(n)",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Неизвестный тип теоретической функции."),
    };
}
