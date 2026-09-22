using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.CustomAlgorithms;

/// <summary>
/// Odd-Even Sort (нечётно-чётная сортировка).
/// Теоретическая сложность: O(n^2).
/// </summary>
public sealed class OddEvenSortAlgorithm : ITimedAlgorithm<double[], double[]>
{
    public string Name => "OddEvenSort";

    public double[] Execute(double[] input)
    {
        var array = (double[])input.Clone();

        var n = array.Length;
        var sorted = false;

        while (!sorted)
        {
            sorted = true;

            // Нечётная фаза:
            // сравниваем пары (1,2), (3,4), (5,6), ...
            for (var i = 1; i < n - 1; i += 2)
            {
                if (array[i] > array[i + 1])
                {
                    (array[i], array[i + 1]) =
                        (array[i + 1], array[i]);

                    sorted = false;
                }
            }

            // Чётная фаза:
            // сравниваем пары (0,1), (2,3), (4,5), ...
            for (var i = 0; i < n - 1; i += 2)
            {
                if (array[i] > array[i + 1])
                {
                    (array[i], array[i + 1]) =
                        (array[i + 1], array[i]);

                    sorted = false;
                }
            }
        }

        return array;
    }
}