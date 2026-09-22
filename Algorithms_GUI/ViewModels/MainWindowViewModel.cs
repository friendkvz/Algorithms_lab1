using System.Collections.ObjectModel;
using Algorithms_programm.Approximation;
using Algorithms_programm.Database.Repositories;
using Algorithms_programm.Models;
using Algorithms_programm.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace Algorithms_GUI;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly ExperimentOrchestrator _orchestrator;
    private readonly ComparisonService _comparison;
    private CancellationTokenSource? _cts;
    private IAlgorithmRunner? _selectedAlgorithm;
    private int _nMax, _step = 100, _mMax = 100, _mStep = 25, _runs = 5;
    private bool _force, _isBusy;
    private double _progress;
    private string _status = "Готово";
    private PlotModel? _plot;
    private MatrixExperimentResultDto? _matrixResult;

    public ObservableCollection<IAlgorithmRunner> Algorithms { get; }
    public ObservableCollection<ApproximationResult> Approximations { get; } = new();
    public ObservableCollection<SessionSummaryDto> Sessions { get; } = new();
    public IAlgorithmRunner? SelectedAlgorithm { get => _selectedAlgorithm; set { if (Set(ref _selectedAlgorithm, value) && value != null) NMax = value.DefaultNMax; } }
    public int NMax { get => _nMax; set => Set(ref _nMax, value); }
    public int Step { get => _step; set => Set(ref _step, value); }
    public int MMax { get => _mMax; set => Set(ref _mMax, value); }
    public int MStep { get => _mStep; set => Set(ref _mStep, value); }
    public int Runs { get => _runs; set => Set(ref _runs, value); }
    public bool ForceRecalculate { get => _force; set => Set(ref _force, value); }
    public bool IsBusy { get => _isBusy; private set => Set(ref _isBusy, value); }
    public double Progress { get => _progress; private set => Set(ref _progress, value); }
    public string Status { get => _status; private set => Set(ref _status, value); }
    public PlotModel? Plot { get => _plot; private set => Set(ref _plot, value); }
    public AsyncCommand RunCommand { get; }
    public AsyncCommand CancelCommand { get; }
    public AsyncCommand LoadSessionsCommand { get; }
    public AsyncCommand CompareCommand { get; }

    public MainWindowViewModel(AlgorithmRegistry registry, ExperimentOrchestrator orchestrator, ComparisonService comparison)
    {
        _orchestrator = orchestrator; _comparison = comparison;
        Algorithms = new(registry.All); SelectedAlgorithm = Algorithms.FirstOrDefault();
        RunCommand = new(RunAsync, () => !IsBusy);
        CancelCommand = new(() => { _cts?.Cancel(); return Task.CompletedTask; }, () => IsBusy);
        LoadSessionsCommand = new(LoadSessionsAsync, () => SelectedAlgorithm != null && !IsBusy);
        CompareCommand = new(CompareAsync, () => Sessions.Count > 0 && !IsBusy);
    }

    private void Validate()
    {
        if (SelectedAlgorithm == null) throw new InvalidOperationException("Выберите алгоритм.");
        if (NMax < 1 || Step < 1 || Runs < 1 || MMax < 1 || MStep < 1) throw new ArgumentException("Все параметры должны быть положительными.");
        if (SelectedAlgorithm.IsStepCounting && NMax > 1000) throw new ArgumentException("Для возведения в степень n ограничено диапазоном 1..1000.");
    }

    private async Task RunAsync()
    {
        try
        {
            Validate(); IsBusy = true; Progress = 0; Status = "Выполнение эксперимента..."; _cts = new();
            var progress = new Progress<double>(p => Progress = p * 100);
            if (SelectedAlgorithm!.IsMatrix)
            {
                _matrixResult = await _orchestrator.RunMatrixExperimentAsync(SelectedAlgorithm.Name, NMax, Step, MMax, MStep, Runs, ForceRecalculate, progress, _cts.Token);
                Plot = BuildMatrixPlot(_matrixResult);
                Approximations.Clear();
            }
            else
            {
                var result = await _orchestrator.RunExperimentAsync(SelectedAlgorithm.Name, NMax, Step, Runs, ForceRecalculate, progress, _cts.Token);
                Approximations.Clear(); foreach (var a in result.Approximations) Approximations.Add(a);
                Plot = BuildPlot(result);
            }
            Status = "Эксперимент завершён.";
        }
        catch (OperationCanceledException) { Status = "Эксперимент отменён."; }
        catch (Exception ex) { Status = $"Ошибка: {ex.Message}"; }
        finally { IsBusy = false; _cts?.Dispose(); _cts = null; }
    }

    private PlotModel BuildPlot(ExperimentResultDto result)
    {
        var model = new PlotModel { Title = $"{result.AlgorithmName} — сессия {result.SessionId}" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "n" });
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = result.Points[0].Kind == MeasureKind.StepCount ? "Число шагов" : "Время, мс" });
        var points = new LineSeries { Title = "Эмпирические точки", MarkerType = MarkerType.Circle };
        foreach (var p in result.Points) points.Points.Add(new DataPoint(p.N, p.MeanValue));
        model.Series.Add(points);
        var fit = result.BestFit; var curve = new LineSeries { Title = $"{fit.DisplayFormula}; MSE={fit.Mse:G4}" };
        var min = result.Points.Min(p => p.N); var max = result.Points.Max(p => p.N);
        for (var i = 0; i <= 100; i++) { var n = min + (max - min) * i / 100.0; curve.Points.Add(new DataPoint(n, fit.Evaluate(n))); }
        model.Series.Add(curve); return model;
    }

    private static PlotModel BuildMatrixPlot(MatrixExperimentResultDto result)
    {
        var model = new PlotModel { Title = $"Матричное умножение — сессия {result.SessionId}" };
        var xs = result.Points.Where(p => p.M.HasValue).Select(p => p.N).Distinct().Order().ToArray();
        var ys = result.Points.Where(p => p.M.HasValue).Select(p => p.M!.Value).Distinct().Order().ToArray();
        var values = new double[ys.Length, xs.Length];
        foreach (var p in result.Points) { var x = Array.IndexOf(xs, p.N); var y = Array.IndexOf(ys, p.M!.Value); if (x >= 0 && y >= 0) values[y, x] = p.MeanValue; }
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "n" }); model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "m" });
        model.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Title = "Время, мс" });
        model.Series.Add(new HeatMapSeries { X0 = xs.FirstOrDefault(), X1 = xs.LastOrDefault(), Y0 = ys.FirstOrDefault(), Y1 = ys.LastOrDefault(), Interpolate = false, Data = values });
        return model;
    }

    private async Task LoadSessionsAsync()
    {
        Sessions.Clear(); if (SelectedAlgorithm == null) return;
        foreach (var s in await _comparison.GetSessionsAsync(SelectedAlgorithm.Name)) Sessions.Add(s);
        Status = $"Загружено сессий: {Sessions.Count}";
    }

    private async Task CompareAsync()
    {
        if (SelectedAlgorithm == null) return;
        var model = new PlotModel { Title = $"Сравнение сессий: {SelectedAlgorithm.Name}" };
        model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "n" }); model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Значение" });
        foreach (var session in Sessions)
        {
            var series = new LineSeries { Title = session.DisplayName };
            foreach (var p in await _comparison.GetSessionPointsAsync(session.SessionId)) series.Points.Add(new DataPoint(p.N, p.MeanValue));
            model.Series.Add(series);
        }
        Plot = model; Status = "Сессии сравнены.";
    }
}
