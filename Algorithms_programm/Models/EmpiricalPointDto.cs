namespace Algorithms_programm.Models;

/// <summary>
/// Усреднённая эмпирическая точка для GUI: n (и m для матричного умножения), среднее
/// измеренное значение (мс времени либо число шагов — MeasureKind уточняет какое) и сколько
/// запусков вошло в среднее (обычно RunsPerPoint сессии, если точка не была докачана частично).
/// </summary>
public sealed record EmpiricalPointDto(int N, int? M, double MeanValue, int RunCount, MeasureKind Kind);

public enum MeasureKind
{
    /// <summary>MeanValue — среднее время в миллисекундах (Части I–III методички).</summary>
    ElapsedMilliseconds,

    /// <summary>MeanValue — среднее число операций/шагов (Часть IV методички).</summary>
    StepCount,
}
