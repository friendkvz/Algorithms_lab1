using Algorithms_programm.Approximation;

namespace Algorithms_programm.Models;

/// <summary>
/// Результат одного полного прогона эксперимента (Части I, III, IV методички — всё, кроме
/// матричного умножения, у которого отдельный DTO из-за второй размерности m).
/// Approximations отсортированы по возрастанию MSE — Approximations[0] совпадает с BestFit
/// и является тем, что GUI показывает как "автоматически подобранную" кривую по умолчанию;
/// пользователь может переключиться на любую другую функцию из Approximations на графике.
/// </summary>
public sealed record ExperimentResultDto(
    string AlgorithmName,
    int SessionId,
    IReadOnlyList<EmpiricalPointDto> Points,
    IReadOnlyList<ApproximationResult> Approximations)
{
    public ApproximationResult BestFit => Approximations[0];
}
