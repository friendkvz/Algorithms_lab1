using Algorithms_programm.Approximation;

namespace Algorithms_programm.Models;

/// <summary>
/// Историческая сессия эксперимента вместе с её усреднёнными эмпирическими точками — то, что
/// показывает окно сравнения нескольких сохранённых экспериментов (Этап 4 методички,
/// "данные берутся из БД"). Label — то, что пользователь может задать при запуске, чтобы потом
/// отличать сессии друг от друга в списке для сравнения (например "до оптимизации" / "после").
/// </summary>
public sealed record SessionSummary(
    int SessionId,
    string AlgorithmName,
    DateTime CreatedAt,
    int NMax,
    int Step,
    int RunsPerPoint,
    string? Label,
    IReadOnlyList<DataPoint> EmpiricalPoints);
