namespace Algorithms_programm.Database.Entities;

/// <summary>
/// Один конкретный замер: один из 5 (или сколько задано в сессии) независимых запусков
/// алгоритма при заданном n (и m для матриц). Хранится "сырьём", а не только средним —
/// это то, что явно требует методичка ("номер запуска" в списке хранимых полей), и позволяет
/// пересчитать среднее/отбросить выбросы позже без повторного запуска алгоритма.
///
/// Ровно одно из полей ElapsedMilliseconds / StepCount заполнено:
///  - ElapsedMilliseconds — для алгоритмов, где методичка требует время (Части I–III);
///  - StepCount — для алгоритмов возведения в степень (Часть IV), где методичка явно
///    требует считать число операций, а не время.
/// </summary>
public class ExperimentRunEntity
{
    public int Id { get; set; }

    public int SessionId { get; set; }
    public ExperimentSessionEntity Session { get; set; } = null!;

    public int N { get; set; }

    /// <summary>Только для матричного умножения: вторая размерность m.</summary>
    public int? M { get; set; }

    /// <summary>Номер запуска в пределах точки (n, m): 1..RunsPerPoint сессии.</summary>
    public int RunIndex { get; set; }

    public double? ElapsedMilliseconds { get; set; }

    public long? StepCount { get; set; }

    public DateTime MeasuredAt { get; set; }
}
