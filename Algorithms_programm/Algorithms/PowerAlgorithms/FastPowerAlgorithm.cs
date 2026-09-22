using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.PowerAlgorithms;

/// <summary>
/// Алгоритм 12 методички: быстрое (бинарное) возведение в степень — деление показателя
/// степени пополам на каждом шаге. Теоретическая сложность по числу операций — O(log n).
/// </summary>
public sealed class FastPowerAlgorithm : IStepCountingAlgorithm<PowerInput, double>
{
    public string Name => "FastPower";

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

        var (halfResult, halfSteps) = Power(x, n / 2);
        var squared = halfResult * halfResult;
        var steps = halfSteps + 1;

        if (n % 2 == 1)
        {
            squared *= x;
            steps++;
        }

        return (squared, steps);
    }
}
