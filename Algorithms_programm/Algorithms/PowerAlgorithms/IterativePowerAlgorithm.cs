using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.PowerAlgorithms;

/// <summary>
/// Алгоритм 10 методички: простой итеративный алгоритм x^n за n-1 умножений
/// (result = x, затем умножаем на x ещё (n-1) раз). Теоретическая сложность по числу
/// операций — O(n).
/// </summary>
public sealed class IterativePowerAlgorithm : IStepCountingAlgorithm<PowerInput, double>
{
    public string Name => "IterativePower";

    public (double Result, long Steps) ExecuteCountingSteps(PowerInput input)
    {
        if (input.N < 0)
        {
            throw new ArgumentException("Показатель степени N не может быть отрицательным.", nameof(input));
        }

        if (input.N == 0)
        {
            return (1.0, 0);
        }

        var result = input.X;
        long steps = 0;
        for (var i = 2; i <= input.N; i++)
        {
            result *= input.X;
            steps++;
        }

        return (result, steps);
    }
}
