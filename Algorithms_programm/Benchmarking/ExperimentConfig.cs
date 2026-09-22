using System.Security.Cryptography;
using System.Text;

namespace Algorithms_programm.Benchmarking;

/// <summary>
/// Конфигурация эксперимента по методичке: N_max (настраиваемо в GUI, дефолт зависит
/// от категории сложности алгоритма), шаг перебора n, число независимых запусков на точку
/// (по методичке — 5), и опционально M_max/M_step для матричного умножения (2D-перебор n и m
/// для 3D-графика). ForceRecalculate сюда намеренно не входит: это не часть "конфигурации"
/// эксперимента как таковой, а разовый флаг поведения кэша при конкретном запуске.
/// </summary>
public sealed record ExperimentConfig(
    string AlgorithmName,
    int NMax,
    int Step,
    int RunsPerPoint,
    int? MMax = null,
    int? MStep = null)
{
    /// <summary>
    /// Стабильный хэш всех полей конфигурации (кроме AlgorithmName, который хранится отдельно
    /// как внешний ключ на AlgorithmEntity). Две сессии одного алгоритма с одинаковым хэшем
    /// считаются кэш-эквивалентными: DbBackedCacheService переиспользует данные вместо
    /// повторного запуска алгоритма, если пользователь не запросил force recalculate.
    /// </summary>
    public string ComputeConfigHash()
    {
        var raw = $"{NMax}|{Step}|{RunsPerPoint}|{MMax}|{MStep}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes);
    }

    /// <summary>Последовательность n = 1, 1+Step, 1+2*Step, ... не превышающих NMax (включая NMax, если попадает в шаг).</summary>
    public IEnumerable<int> EnumerateN() => EnumerateRange(NMax, Step);

    /// <summary>
    /// Общая логика построения сетки "1, 1+step, 1+2*step, ..., max" (гарантированно включая max).
    /// Вынесена как static, чтобы ExperimentOrchestrator мог построить такую же сетку по m
    /// (MMax/MStep) для матричного умножения, не создавая для этого второй фиктивный ExperimentConfig.
    /// </summary>
    public static IEnumerable<int> EnumerateRange(int max, int step)
    {
        if (step < 1)
        {
            throw new InvalidOperationException("Step должен быть положительным (>= 1).");
        }

        if (max < 1)
        {
            yield break;
        }

        for (var n = 1; n <= max; n += step)
        {
            yield return n;
        }

        // Гарантируем, что верхняя граница max всегда присутствует на графике, даже если
        // она не попадает точно в сетку "1, 1+step, 1+2*step, ...".
        var lastYielded = 1 + ((max - 1) / step) * step;
        if (lastYielded != max)
        {
            yield return max;
        }
    }
}
