using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.VectorOperations;

/// <summary>
/// Алгоритм 5 методички: сортировка пузырьком. Теоретическая сложность O(n^2).
/// Сортирует копию входного массива, чтобы не мутировать общие тестовые/сгенерированные данные
/// между повторными запусками бенчмарка (5 независимых запусков на одном n должны получать
/// одинаковый несортированный вход).
/// </summary>
public sealed class BubbleSortAlgorithm : ITimedAlgorithm<double[], double[]>
{
    public string Name => "BubbleSort";

    public double[] Execute(double[] input)
    {
        var array = (double[])input.Clone();
        var n = array.Length;
        for (var i = 0; i < n - 1; i++)
        {
            var swapped = false;
            for (var j = 0; j < n - 1 - i; j++)
            {
                if (array[j] > array[j + 1])
                {
                    (array[j], array[j + 1]) = (array[j + 1], array[j]);
                    swapped = true;
                }
            }

            if (!swapped)
            {
                break;
            }
        }

        return array;
    }
}
