using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Algorithms_programm.Approximation;
using Algorithms_programm.Models;
using Algorithms_programm.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyDataPoint = OxyPlot.DataPoint;

namespace Algorithms_GUI;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly ExperimentOrchestrator _orchestrator;
    private readonly ComparisonService _comparisonService;

    private CancellationTokenSource? _cancellationTokenSource;
    private IAlgorithmRunner? _selectedAlgorithm;

    private bool _isMatrixAlgorithm;
    private int _nMax;
    private int _step = 100;
    private int _mMax = 100;
    private int _mStep = 25;
    private int _runs = 5;

    private bool _forceRecalculate;
    private bool _isBusy;
    private double _progress;
    private string _status = "Готово";
    private PlotModel? _plot;
    private MatrixExperimentResultDto? _matrixResult;

    public MainWindowViewModel(
        AlgorithmRegistry registry,
        ExperimentOrchestrator orchestrator,
        ComparisonService comparisonService)
    {
        _orchestrator = orchestrator;
        _comparisonService = comparisonService;

        Algorithms = new ObservableCollection<IAlgorithmRunner>(
            registry.All.OrderBy(algorithm => algorithm.Name));

        RunCommand = new AsyncCommand(
            RunExperimentAsync,
            () => !IsBusy && SelectedAlgorithm is not null);

        CancelCommand = new AsyncCommand(
            CancelExperimentAsync,
            () => IsBusy);

        LoadSessionsCommand = new AsyncCommand(
            LoadSessionsAsync,
            () => !IsBusy && SelectedAlgorithm is not null);

        CompareCommand = new AsyncCommand(
            CompareSessionsAsync,
            () => !IsBusy && Sessions.Count > 0);

        SelectedAlgorithm = Algorithms.FirstOrDefault();
    }

    public ObservableCollection<IAlgorithmRunner> Algorithms { get; }

    public ObservableCollection<ApproximationResult> Approximations { get; } = new();

    public ObservableCollection<SessionSummaryDto> Sessions { get; } = new();

    public IAlgorithmRunner? SelectedAlgorithm
    {
        get => _selectedAlgorithm;
        set
        {
            if (!Set(ref _selectedAlgorithm, value) || value is null)
            {
                return;
            }

            NMax = value.DefaultNMax;
            IsMatrixAlgorithm = value.IsMatrix;
            Approximations.Clear();
            Sessions.Clear();
            Plot = null;

            RunCommand.RaiseCanExecuteChanged();
            LoadSessionsCommand.RaiseCanExecuteChanged();
        }
    }

    public int NMax
    {
        get => _nMax;
        set => Set(ref _nMax, value);
    }

    public int Step
    {
        get => _step;
        set => Set(ref _step, value);
    }

    public int MMax
    {
        get => _mMax;
        set => Set(ref _mMax, value);
    }

    public int MStep
    {
        get => _mStep;
        set => Set(ref _mStep, value);
    }

    public int Runs
    {
        get => _runs;
        set => Set(ref _runs, value);
    }

    public bool ForceRecalculate
    {
        get => _forceRecalculate;
        set => Set(ref _forceRecalculate, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (!Set(ref _isBusy, value))
            {
                return;
            }

            RunCommand.RaiseCanExecuteChanged();
            CancelCommand.RaiseCanExecuteChanged();
            LoadSessionsCommand.RaiseCanExecuteChanged();
            CompareCommand.RaiseCanExecuteChanged();
        }
    }

    public double Progress
    {
        get => _progress;
        private set => Set(ref _progress, value);
    }

    public string Status
    {
        get => _status;
        set => Set(ref _status, value);
    }
    public bool IsMatrixAlgorithm
    {
        get => _isMatrixAlgorithm;
        private set => Set(ref _isMatrixAlgorithm, value);
    }

    public PlotModel? Plot
    {
        get => _plot;
        private set => Set(ref _plot, value);
    }
    
    public MatrixExperimentResultDto? MatrixResult
    {
        get => _matrixResult;
        private set => Set(ref _matrixResult, value);
    }

    public AsyncCommand RunCommand { get; }

    public AsyncCommand CancelCommand { get; }

    public AsyncCommand LoadSessionsCommand { get; }

    public AsyncCommand CompareCommand { get; }

    private void ValidateParameters()
    {
        if (SelectedAlgorithm is null)
        {
            throw new InvalidOperationException("Выберите алгоритм.");
        }

        if (NMax < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(NMax), "N_max должен быть положительным.");
        }

        if (Step < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(Step), "Шаг должен быть положительным.");
        }

        if (Runs < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(Runs), "Количество запусков должно быть положительным.");
        }

        if (MMax < 1 || MStep < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MMax),
                "Параметры матриц должны быть положительными.");
        }

        if (SelectedAlgorithm.IsStepCounting && NMax > 1000)
        {
            throw new ArgumentOutOfRangeException(
                nameof(NMax),
                "Для алгоритмов возведения в степень N_max не может превышать 1000.");
        }

        if (SelectedAlgorithm.IsMatrix && MMax < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MMax),
                "M_max должен быть положительным.");
        }
    }

    private async Task RunExperimentAsync()
    {
        try
        {
            ValidateParameters();

            IsBusy = true;
            Progress = 0;
            Status = "Выполнение эксперимента...";

            _cancellationTokenSource = new CancellationTokenSource();

            var progress = new Progress<double>(
                value => Progress = Math.Clamp(value * 100.0, 0.0, 100.0));

            if (SelectedAlgorithm!.IsMatrix)
            {
                var matrixResult =
                    await _orchestrator.RunMatrixExperimentAsync(
                        SelectedAlgorithm.Name,
                        NMax,
                        Step,
                        MMax,
                        MStep,
                        Runs,
                        ForceRecalculate,
                        progress,
                        _cancellationTokenSource.Token);

                Approximations.Clear();
                MatrixResult = matrixResult;
                Plot = null;
            }
            else
            {
                var result =
                    await _orchestrator.RunExperimentAsync(
                        SelectedAlgorithm.Name,
                        NMax,
                        Step,
                        Runs,
                        ForceRecalculate,
                        progress,
                        _cancellationTokenSource.Token);

                Approximations.Clear();

                foreach (var approximation in result.Approximations)
                {
                    Approximations.Add(approximation);
                }

                Plot = BuildExperimentPlot(result);
            }

            Progress = 100;
            Status = "Эксперимент завершён.";
        }
        catch (OperationCanceledException)
        {
            Status = "Эксперимент отменён.";
        }
        catch (Exception exception)
        {
            Status = $"Ошибка: {exception.Message}";
        }
        finally
        {
            IsBusy = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private Task CancelExperimentAsync()
    {
        _cancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }

    private PlotModel BuildExperimentPlot(ExperimentResultDto result)
    {
        var model = new PlotModel
        {
            Title = $"{result.AlgorithmName} — сессия {result.SessionId}"
        };

        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "n"
        });

        var valueTitle =
            result.Points.FirstOrDefault()?.Kind == MeasureKind.StepCount
                ? "Количество операций"
                : "Время, мс";

        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = valueTitle
        });

        var empiricalSeries = new LineSeries
        {
            Title = "Эмпирические точки",
            MarkerType = MarkerType.Circle,
            MarkerSize = 3
        };

        foreach (var point in result.Points)
        {
            empiricalSeries.Points.Add(
                new OxyDataPoint(point.N, point.MeanValue));
        }

        model.Series.Add(empiricalSeries);

        if (result.Points.Count == 0 || result.Approximations.Count == 0)
        {
            return model;
        }

        var bestFit = result.BestFit;

        var approximationSeries = new LineSeries
        {
            Title = $"{bestFit.DisplayFormula}; MSE={bestFit.Mse:G4}"
        };

        var minimumN = result.Points.Min(point => point.N);
        var maximumN = result.Points.Max(point => point.N);

        for (var index = 0; index <= 100; index++)
        {
            var n = minimumN +
                    (maximumN - minimumN) * index / 100.0;

            approximationSeries.Points.Add(
                new OxyDataPoint(n, bestFit.Evaluate(n)));
        }

        model.Series.Add(approximationSeries);

        return model;
    }

    private static PlotModel BuildMatrixPlot(MatrixExperimentResultDto result)
    {
        var model = new PlotModel
        {
            Title = $"Матричное умножение — сессия {result.SessionId}"
        };

        var matrixPoints = result.Points
            .Where(point => point.M.HasValue)
            .ToList();

        if (matrixPoints.Count == 0)
        {
            return model;
        }

        var xValues = matrixPoints
            .Select(point => point.N)
            .Distinct()
            .OrderBy(value => value)
            .ToArray();

        var yValues = matrixPoints
            .Select(point => point.M!.Value)
            .Distinct()
            .OrderBy(value => value)
            .ToArray();

        var values = new double[yValues.Length, xValues.Length];

        foreach (var point in matrixPoints)
        {
            var xIndex = Array.IndexOf(xValues, point.N);
            var yIndex = Array.IndexOf(yValues, point.M!.Value);

            if (xIndex >= 0 && yIndex >= 0)
            {
                values[yIndex, xIndex] = point.MeanValue;
            }
        }

        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "n"
        });

        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "m"
        });

        model.Axes.Add(new LinearColorAxis
        {
            Position = AxisPosition.Right,
            Title = "Время, мс"
        });

        model.Series.Add(new HeatMapSeries
        {
            X0 = xValues.First(),
            X1 = xValues.Last(),
            Y0 = yValues.First(),
            Y1 = yValues.Last(),
            Interpolate = false,
            Data = values
        });

        return model;
    }

    private async Task LoadSessionsAsync()
    {
        if (SelectedAlgorithm is null)
        {
            return;
        }

        try
        {
            IsBusy = true;
            Status = "Загрузка исторических сессий...";

            Sessions.Clear();

            var sessions =
                await _comparisonService.GetSessionsAsync(
                    SelectedAlgorithm.Name);

            foreach (var session in sessions)
            {
                Sessions.Add(session);
            }

            Status = $"Загружено сессий: {Sessions.Count}";
        }
        catch (Exception exception)
        {
            Status = $"Ошибка загрузки сессий: {exception.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task CompareSessionsAsync()
    {
        if (SelectedAlgorithm is null || Sessions.Count == 0)
        {
            return;
        }

        try
        {
            IsBusy = true;
            Status = "Построение сравнения сессий...";

            var model = new PlotModel
            {
                Title = $"Сравнение сессий: {SelectedAlgorithm.Name}"
            };

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "n"
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Среднее значение"
            });

            foreach (var session in Sessions)
            {
                var series = new LineSeries
                {
                    Title = session.DisplayName
                };

                var points =
                    await _comparisonService.GetSessionPointsAsync(
                        session.SessionId);

                foreach (var point in points)
                {
                    series.Points.Add(
                        new OxyDataPoint(point.N, point.MeanValue));
                }

                model.Series.Add(series);
            }

            Plot = model;
            Status = "Сравнение сессий построено.";
        }
        catch (Exception exception)
        {
            Status = $"Ошибка сравнения: {exception.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}