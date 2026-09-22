using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.VectorOperations;

/// <summary>
/// Алгоритм 7 методички: встроенная сортировка языка (Array.Sort). В .NET это introsort
/// (гибрид quicksort/heapsort/insertion sort), а не classic Timsort, но по роли в методичке
/// ("встроенная сортировка платформы") — это её прямой аналог. Теоретическая сложность O(n log n).
/// </summary>
public sealed class BuiltInSortAlgorithm : ITimedAlgorithm<double[], double[]>
{
    public string Name => "BuiltInSort";

    public double[] Execute(double[] input)
    {
        var array = (double[])input.Clone();
        Array.Sort(array);
        return array;
    }
}
