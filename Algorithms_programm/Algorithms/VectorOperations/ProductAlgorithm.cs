using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.VectorOperations;

/// <summary>
/// Алгоритм 3 методички: произведение элементов вектора. Теоретическая сложность O(n).
/// </summary>
public sealed class ProductAlgorithm : ITimedAlgorithm<double[], double>
{
    public string Name => "Product";

    public double Execute(double[] input)
    {
        double product = 1;
        for (var i = 0; i < input.Length; i++)
        {
            product *= input[i];
        }

        return product;
    }
}
