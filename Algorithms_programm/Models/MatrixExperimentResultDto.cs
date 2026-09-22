namespace Algorithms_programm.Models;

/// <summary>
/// Результат эксперимента для матричного умножения (алгоритм 8): плоский список точек
/// (n, m, среднее время), достаточный и для heatmap, и для полноценного 3D-графика, если
/// GUI решит его строить. Методичка не требует аппроксимации теоретической функцией именно
/// для этого 3D-случая (там просто время(n, m)), поэтому Approximation-результатов здесь нет.
/// </summary>
public sealed record MatrixExperimentResultDto(
    int SessionId,
    IReadOnlyList<EmpiricalPointDto> Points);
