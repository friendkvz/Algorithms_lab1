using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.CustomAlgorithms;

/// <summary>
/// ПРИМЕР для Части III методички (точка расширения).
///
/// Название "Han's sort" в задании отсылает к детерминированному целочисленному алгоритму
/// сортировки Yijie Han (2002–2004), который на теоретическом уровне сортирует n целых чисел
/// за O(n log log n) в word-RAM модели. У этого алгоритма нет практической реализации: он
/// построен на fusion trees и упаковке нескольких ключей в одно машинное слово (packed sorting),
/// и его константы/сложность кода несопоставимы с задачами лабораторной работы.
///
/// Здесь реализована ПРАКТИЧЕСКАЯ техника из той же линии идей — на которой, в частности,
/// строятся более ранние результаты этого направления (Kirkpatrick–Reisch, Andersson):
/// поразрядная (radix) сортировка целых чисел через устойчивую counting-сортировку по группам
/// битов ключа (LSD radix sort, "цифры" по 16 бит). Идея та же — выйти за пределы сравнения
/// пар элементов (нижняя граница comparison-based sort — O(n log n)) за счёт прямой арифметики
/// над битами ключа. Практическая сложность — O(n) для входов ограниченного диапазона
/// (32-битные неотрицательные целые), т.к. используется фиксированное число проходов (2 прохода
/// по 16 бит), а не O(n log log n) — это сознательное упрощение ради работоспособности.
/// </summary>
public sealed class HanSortAlgorithm : ITimedAlgorithm<int[], int[]>
{
    private const int BitsPerDigit = 16;
    private const int DigitCount = 1 << BitsPerDigit; // 65536
    private const int DigitMask = DigitCount - 1;

    public string Name => "HanSort";

    public int[] Execute(int[] input)
    {
        foreach (var value in input)
        {
            if (value < 0)
            {
                throw new ArgumentException(
                    "HanSort работает только с неотрицательными целыми числами (word-RAM модель).",
                    nameof(input));
            }
        }

        var array = input;

        // Два прохода по 16 бит покрывают весь диапазон 32-битного int (16 + 16 = 32 бита).
        array = CountingSortByDigit(array, shift: 0);
        array = CountingSortByDigit(array, shift: BitsPerDigit);

        return array;
    }

    /// <summary>
    /// Устойчивая (stable) counting-сортировка массива по одной "цифре" из BitsPerDigit бит,
    /// извлечённой сдвигом shift. Устойчивость обязательна для корректности LSD radix sort:
    /// порядок, установленный на предыдущих (младших) цифрах, должен сохраняться.
    /// </summary>
    private static int[] CountingSortByDigit(int[] array, int shift)
    {
        var counts = new int[DigitCount + 1];

        for (var i = 0; i < array.Length; i++)
        {
            var digit = (array[i] >> shift) & DigitMask;
            counts[digit + 1]++;
        }

        for (var digit = 0; digit < DigitCount; digit++)
        {
            counts[digit + 1] += counts[digit];
        }

        var output = new int[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            var digit = (array[i] >> shift) & DigitMask;
            output[counts[digit]] = array[i];
            counts[digit]++;
        }

        return output;
    }
}
