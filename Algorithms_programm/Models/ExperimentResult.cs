using Algorithms_programm.Approximation;

namespace Algorithms_programm.Models;

/// <summary>
/// Итог одного вызова ExperimentOrchestrator.RunExperimentAsync — всё, что нужно окну
/// визуализации графика (Этап 4 методички): эмпирические точки, все шесть подобранных
/// теоретических функций (отсортированы по MSE, [0] — лучшая), и id сессии в БД для истории.
/// </summary>
public sealed record ExperimentResult(
    int SessionId,
    string AlgorithmName,
    IReadOnlyList<DataPoint> EmpiricalPoints,
    IReadOnlyList<ApproximationResult> Approximations)
{
    public ApproximationResult BestApproximation => Approximations[0];
}
