namespace Algorithms_programm.Database.Entities;

/// <summary>
/// "Сессия эксперимента" — группа замеров, выполненных с одинаковыми параметрами
/// (N_max, шаг, число запусков на точку, и, для матриц, M_max/M_step). Ровно то, что
/// методичка называет "конфигурацией эксперимента" — нужна, чтобы:
///  1) кэш понимал, можно ли переиспользовать уже посчитанные точки для точно такой же
///     конфигурации, не смешивая их с данными от другой конфигурации того же алгоритма;
///  2) GUI мог сравнивать несколько исторических сессий на одном графике (Этап 4 методички).
/// </summary>
public class ExperimentSessionEntity
{
    public int Id { get; set; }

    public int AlgorithmId { get; set; }
    public AlgorithmEntity Algorithm { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int NMax { get; set; }
    public int Step { get; set; }
    public int RunsPerPoint { get; set; }

    /// <summary>Только для матричного умножения (алгоритм 8): максимум по второй размерности m.</summary>
    public int? MMax { get; set; }

    /// <summary>Только для матричного умножения: шаг по m.</summary>
    public int? MStep { get; set; }

    /// <summary>
    /// Детерминированный хэш всех параметров конфигурации выше (см. ExperimentConfig.ComputeConfigHash).
    /// Две сессии одного алгоритма с одинаковым ConfigHash считаются "той же конфигурацией"
    /// для целей кэширования.
    /// </summary>
    public string ConfigHash { get; set; } = string.Empty;

    /// <summary>Необязательная человекочитаемая метка сессии для окна сравнения в GUI.</summary>
    public string? Label { get; set; }

    public ICollection<ExperimentRunEntity> Runs { get; set; } = new List<ExperimentRunEntity>();
}
