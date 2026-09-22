namespace Algorithms_programm.Models;

/// <summary>
/// Прогресс выполнения эксперимента, отдаётся через IProgress&lt;ExperimentProgress&gt; из
/// ExperimentOrchestrator в GUI (ViewModel), чтобы обновлять прогресс-бар, не блокируя
/// UI-поток WPF во время потенциально многосекундного/многоминутного прогона.
/// </summary>
public readonly record struct ExperimentProgress(int CompletedPoints, int TotalPoints, int CurrentN, int? CurrentM)
{
    public double PercentComplete => TotalPoints == 0 ? 0 : 100.0 * CompletedPoints / TotalPoints;
}
