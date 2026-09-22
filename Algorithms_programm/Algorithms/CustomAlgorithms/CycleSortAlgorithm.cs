using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.CustomAlgorithms;

/// <summary>
/// Сортировка циклом (Cycle Sort)
/// Теоретическая сложность: O(n^2)
/// Особенность: совершает теоретически минимальное количество операций записи в память - O(n)
/// </summary>
public sealed class CycleSortAlgorithm : ITimedAlgorithm<double[], double[]>
{
    public string Name => "CycleSort";

    public double[] Execute(double[] input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var array = (double[])input.Clone();
        var n = array.Length;

        for (var cycleStart = 0; cycleStart < n - 1; cycleStart++)
        {
            var item = array[cycleStart];
            var pos = cycleStart;

            // Ищем позицию, куда должен встать текущий элемент
            for (var i = cycleStart + 1; i < n; i++)
            {
                if (array[i] < item)
                {
                    pos++;
                }
            }

            // Если элемент уже на своем месте, цикл завершен
            if (pos == cycleStart)
            {
                continue;
            }

            // Пропускаем дубликаты
            while (item == array[pos])
            {
                pos++;
            }

            // Помещаем элемент на правильное место
            if (pos != cycleStart)
            {
                (item, array[pos]) = (array[pos], item);
            }

            // Прокручиваем оставшуюся часть цикла
            while (pos != cycleStart)
            {
                pos = cycleStart;

                for (var i = cycleStart + 1; i < n; i++)
                {
                    if (array[i] < item)
                    {
                        pos++;
                    }
                }

                while (item == array[pos])
                {
                    pos++;
                }

                if (item != array[pos])
                {
                    (item, array[pos]) = (array[pos], item);
                }
            }
        }

        return array;
    }
}