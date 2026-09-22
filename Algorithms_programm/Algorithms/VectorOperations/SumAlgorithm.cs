using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.VectorOperations;

/// <summary>
/// Алгоритм 2 методички: сумма элементов вектора. Теоретическая сложность O(n).
/// </summary>
public sealed class SumAlgorithm : ITimedAlgorithm<double[], double>
{
    public string Name => "Sum";

    public double Execute(double[] input)
    {
        double sum = 0;
        for (var i = 0; i < input.Length; i++)
        {
            sum += input[i];
        }

        return sum;
    }
}
