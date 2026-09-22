using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.CustomAlgorithms;

/// <summary>
/// Практическая целочисленная radix-сортировка с двумя проходами по 16 бит.
/// Работает с неотрицательными 32-битными целыми числами.
/// </summary>
public sealed class HanSortAlgorithm : ITimedAlgorithm<int[], int[]>
{
    private const int BitsPerDigit = 16;
    private const int DigitCount = 1 << BitsPerDigit;
    private const int DigitMask = DigitCount - 1;

    public string Name => "HanSort";

    public int[] Execute(int[] input)
    {
        ArgumentNullException.ThrowIfNull(input);

        foreach (var value in input)
        {
            if (value < 0)
            {
                throw new ArgumentException(
                    "HanSort работает только с неотрицательными целыми числами.",
                    nameof(input));
            }
        }

        var result = CountingSortByDigit(input, 0);
        return CountingSortByDigit(result, BitsPerDigit);
    }

    private static int[] CountingSortByDigit(int[] input, int shift)
    {
        var counts = new int[DigitCount + 1];

        foreach (var value in input)
        {
            var digit = (value >> shift) & DigitMask;
            counts[digit + 1]++;
        }

        for (var digit = 0; digit < DigitCount; digit++)
        {
            counts[digit + 1] += counts[digit];
        }

        var output = new int[input.Length];
        foreach (var value in input)
        {
            var digit = (value >> shift) & DigitMask;
            output[counts[digit]++] = value;
        }

        return output;
    }
}
