using Algorithms_programm.Database.Entities;

namespace Algorithms_programm.Database.Repositories;

public interface IExperimentRepository
{
    Task<AlgorithmEntity> GetOrCreateAlgorithmAsync(string algorithmName, CancellationToken ct = default);

    /// <summary>Ищет существующую сессию алгоритма с данной конфигурацией (по хэшу). Null, если не найдена.</summary>
    Task<ExperimentSessionEntity?> FindSessionByConfigAsync(
        string algorithmName, string configHash, CancellationToken ct = default);

    Task<ExperimentSessionEntity> CreateSessionAsync(
        string algorithmName,
        int nMax,
        int step,
        int runsPerPoint,
        string configHash,
        int? mMax = null,
        int? mStep = null,
        string? label = null,
        CancellationToken ct = default);

    Task AddRunsAsync(IEnumerable<ExperimentRunEntity> runs, CancellationToken ct = default);

    /// <summary>Все запуски конкретной точки (n, m) внутри сессии — основа для проверки кэша и среднего.</summary>
    Task<List<ExperimentRunEntity>> GetRunsAsync(
        int sessionId, int n, int? m = null, CancellationToken ct = default);

    /// <summary>Все запуски сессии целиком — для построения графика эмпирических точек.</summary>
    Task<List<ExperimentRunEntity>> GetAllRunsForSessionAsync(int sessionId, CancellationToken ct = default);

    /// <summary>Список сессий алгоритма (для окна сравнения нескольких исторических экспериментов).</summary>
    Task<List<ExperimentSessionEntity>> GetSessionsForAlgorithmAsync(
        string algorithmName, CancellationToken ct = default);

    /// <summary>Удаляет сессию и все её запуски — используется при force-recalculate.</summary>
    Task DeleteSessionAsync(int sessionId, CancellationToken ct = default);
}
