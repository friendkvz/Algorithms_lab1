using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.VectorOperations;

/// <summary>
/// Алгоритм 4a методички: вычисление многочлена P(x) = v_1 + v_2*x + v_3*x^2 + ... в точке
/// x = 1.5 "наивным" способом — степень x^(k-1) для каждого члена пересчитывается заново
/// через Math.Pow. Теоретическая сложность O(n) с бОльшим по сравнению с методом Горнера
/// константным множителем (каждый Math.Pow сам по себе не O(1) на большинстве рантаймов,
/// но по методичке рассматривается как элементарная операция для оценки на фоне Горнера).
/// </summary>
public sealed class PolynomialNaiveAlgorithm : ITimedAlgorithm<double[], double>
{
    private const double X = 1.5;

    public string Name => "PolynomialNaive";

    public double Execute(double[] input)
    {
        double result = 0;
        for (var k = 0; k < input.Length; k++)
        {
            result += input[k] * Math.Pow(X, k);
        }

        return result;
    }
}
