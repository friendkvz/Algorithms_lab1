using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.VectorOperations;

/// <summary>
/// Алгоритм 6 методички: быстрая сортировка (Quick sort). Теоретическая сложность O(n log n)
/// в среднем случае, O(n^2) в худшем. Пивот выбирается случайно, чтобы избежать
/// систематического худшего случая на уже отсортированных/специально сгенерированных входах.
/// </summary>
public sealed class QuickSortAlgorithm : ITimedAlgorithm<double[], double[]>
{
    private readonly Random _random = new();

    public string Name => "QuickSort";

    public double[] Execute(double[] input)
    {
        var array = (double[])input.Clone();
        if (array.Length > 1)
        {
            Sort(array, 0, array.Length - 1);
        }

        return array;
    }

    private void Sort(double[] array, int low, int high)
    {
        while (low < high)
        {
            var pivotIndex = Partition(array, low, high);

            // Рекурсия по меньшей части, цикл по большей — ограничивает глубину стека до O(log n).
            if (pivotIndex - low < high - pivotIndex)
            {
                Sort(array, low, pivotIndex - 1);
                low = pivotIndex + 1;
            }
            else
            {
                Sort(array, pivotIndex + 1, high);
                high = pivotIndex - 1;
            }
        }
    }

    private int Partition(double[] array, int low, int high)
    {
        var randomPivotIndex = low + _random.Next(high - low + 1);
        (array[randomPivotIndex], array[high]) = (array[high], array[randomPivotIndex]);

        var pivot = array[high];
        var i = low - 1;
        for (var j = low; j < high; j++)
        {
            if (array[j] <= pivot)
            {
                i++;
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        (array[i + 1], array[high]) = (array[high], array[i + 1]);
        return i + 1;
    }
}
