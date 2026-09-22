using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.VectorOperations;

/// <summary>
/// Алгоритм 4b методички: вычисление многочлена P(x) в точке x = 1.5 методом Горнера.
/// P(x) = v_1 + x*(v_2 + x*(v_3 + ... + x*v_n)). Теоретическая сложность O(n), но с меньшим
/// константным множителем, чем у наивного метода (нет Math.Pow, только умножения и сложения).
/// </summary>
public sealed class PolynomialHornerAlgorithm : ITimedAlgorithm<double[], double>
{
    private const double X = 1.5;

    public string Name => "PolynomialHorner";

    public double Execute(double[] input)
    {
        double result = 0;
        for (var k = input.Length - 1; k >= 0; k--)
        {
            result = result * X + input[k];
        }

        return result;
    }
}
