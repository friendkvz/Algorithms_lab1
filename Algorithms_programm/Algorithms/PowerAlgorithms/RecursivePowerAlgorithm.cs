using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.PowerAlgorithms;

/// <summary>
/// Алгоритм 11 методички: рекурсивный алгоритм x^n = x * x^(n-1). Теоретическая сложность
/// по числу операций — O(n), глубина рекурсии тоже O(n) (для n до 1000 стек выдерживает без проблем).
/// </summary>
public sealed class RecursivePowerAlgorithm : IStepCountingAlgorithm<PowerInput, double>
{
    public string Name => "RecursivePower";

    public (double Result, long Steps) ExecuteCountingSteps(PowerInput input)
    {
        if (input.N < 0)
        {
            throw new ArgumentException("Показатель степени N не может быть отрицательным.", nameof(input));
        }

        return Power(input.X, input.N);
    }

    private static (double Result, long Steps) Power(double x, int n)
    {
        if (n == 0)
        {
            return (1.0, 0);
        }

        var (subResult, subSteps) = Power(x, n - 1);
        return (x * subResult, subSteps + 1);
    }
}
