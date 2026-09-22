namespace Algorithms_programm.Models;

/// <summary>Одна точка 3D-развёртки время(n, m) для матричного умножения (алгоритм 8, Этап 4 методички).</summary>
public readonly record struct MatrixSweepPoint(int N, int M, double AverageElapsedMilliseconds);

/// <summary>
/// Итог ExperimentOrchestrator.RunMatrixSweepAsync — сетка точек (n, m) → среднее время.
/// Аппроксимация здесь не строится (f(n) из Approximation — функция одной переменной,
/// а не двух), GUI визуализирует эту сетку как 3D-поверхность или heatmap (см. комментарий
/// в MatrixMultiplicationVisualizationView).
/// </summary>
public sealed record MatrixSweepResult(int SessionId, IReadOnlyList<MatrixSweepPoint> Points);
