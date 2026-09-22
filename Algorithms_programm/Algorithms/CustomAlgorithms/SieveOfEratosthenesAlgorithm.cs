using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.CustomAlgorithms;

/// <summary>
/// ПРИМЕР для Части III методички (точка расширения, не обязательное задание).
/// Решето Эратосфена: подсчёт количества простых чисел не превышающих n.
/// Теоретическая сложность O(n log log n), близка к O(n) — иллюстрирует, что в графике
/// аппроксимации это ближе всего ляжет на кривую n или n*log(n) в зависимости от диапазона n.
/// Вход — просто n (как int), поэтому используется отдельный лёгкий DTO-обёртка не нужна:
/// int реализует нужный контракт напрямую.
/// </summary>
public sealed class SieveOfEratosthenesAlgorithm : ITimedAlgorithm<int, int>
{
    public string Name => "SieveOfEratosthenes_PrimeCount";

    public int Execute(int input)
    {
        if (input < 2)
        {
            return 0;
        }

        var isComposite = new bool[input + 1];
        var count = 0;

        for (var i = 2; i <= input; i++)
        {
            if (isComposite[i])
            {
                continue;
            }

            count++;
            for (long j = (long)i * i; j <= input; j += i)
            {
                isComposite[j] = true;
            }
        }

        return count;
    }
}
