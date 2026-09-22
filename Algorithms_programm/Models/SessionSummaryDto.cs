namespace Algorithms_programm.Models;

/// <summary>
/// Краткое описание сохранённой сессии для окна сравнения ("Этап 4: сравнить несколько
/// исторических экспериментов на одном графике"). Достаточно, чтобы показать список сессий
/// пользователю до того, как он выберет, какие именно подгружать целиком.
/// </summary>
public sealed record SessionSummaryDto(
    int SessionId,
    string AlgorithmName,
    DateTime CreatedAt,
    int NMax,
    int Step,
    int RunsPerPoint,
    int? MMax,
    int? MStep,
    string? Label)
{
    public string DisplayName =>
        Label is { Length: > 0 }
            ? Label
            : $"{AlgorithmName} — {CreatedAt:g} (N_max={NMax}, шаг={Step})";
}
