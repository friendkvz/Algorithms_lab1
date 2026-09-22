using Algorithms_programm.Benchmarking;
using Algorithms_programm.Database.Entities;

namespace Algorithms_programm.Caching;

public interface ICacheService
{
    /// <summary>
    /// Возвращает id сессии для данной конфигурации: существующую (переиспользуется, кэш-хит)
    /// либо новую. Если forceRecalculate == true и подходящая сессия уже существует, старая
    /// сессия (и все её запуски) удаляется и создаётся новая — "принудительный пересчёт"
    /// из методички.
    /// </summary>
    Task<int> GetOrCreateSessionAsync(ExperimentConfig config, bool forceRecalculate, CancellationToken ct = default);

    /// <summary>
    /// Уже сохранённые запуски точки (n, m) в данной сессии. Пустой список — значит, для этой
    /// точки замеров ещё нет и её нужно посчитать.
    /// </summary>
    Task<IReadOnlyList<ExperimentRunEntity>> GetCachedRunsAsync(
        int sessionId, int n, int? m = null, CancellationToken ct = default);

    /// <summary>
    /// True, если для точки (n, m) в сессии уже есть достаточно запусков (>= RunsPerPoint
    /// сессии) и её можно не пересчитывать.
    /// </summary>
    Task<bool> IsPointCachedAsync(int sessionId, int n, int requiredRuns, int? m = null, CancellationToken ct = default);

    Task SaveRunsAsync(int sessionId, IEnumerable<ExperimentRunEntity> runs, CancellationToken ct = default);
}
