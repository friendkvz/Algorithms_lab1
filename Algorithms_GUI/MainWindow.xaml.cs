using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Algorithms_programm.Models;
using HelixToolkit.Wpf;


namespace Algorithms_GUI;

public partial class MainWindow : Window
{
    private const double BoxWidthN = 10.0;
    private const double BoxLengthM = 10.0;
    private const double BoxHeightT = 6.0;

    private double _minN, _maxN;
    private double _minM, _maxM;
    private double _maxZ = 1.0;

    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyPropertyChanged oldViewModel)
        {
            oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.NewValue is INotifyPropertyChanged newViewModel)
        {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainWindowViewModel.MatrixResult))
        {
            return;
        }

        if (sender is MainWindowViewModel viewModel)
        {
            BuildMatrixSurface(viewModel.MatrixResult);
        }
    }

    private void BuildMatrixSurface(MatrixExperimentResultDto? result)
    {
        MatrixSurface.MeshGeometry = null;
        AxesContainer.Children.Clear();

        if (result is null || result.Points.Count == 0)
        {
            return;
        }

        var matrixPoints = result.Points
            .Where(point => point.M.HasValue)
            .ToList();

        if (matrixPoints.Count == 0)
        {
            return;
        }

        var xValues = matrixPoints
            .Select(point => (double)point.N)
            .Distinct()
            .OrderBy(v => v)
            .ToArray();

        var yValues = matrixPoints
            .Select(point => (double)point.M!.Value)
            .Distinct()
            .OrderBy(v => v)
            .ToArray();

        if (xValues.Length < 2 || yValues.Length < 2)
        {
            return;
        }

        _minN = xValues.First();
        _maxN = xValues.Last();
        _minM = yValues.First();
        _maxM = yValues.Last();

        var values = new double[yValues.Length, xValues.Length];

        foreach (var point in matrixPoints)
        {
            int xIndex = Array.IndexOf(xValues, (double)point.N);
            int yIndex = Array.IndexOf(yValues, (double)point.M!.Value);

            if (xIndex >= 0 && yIndex >= 0)
            {
                values[yIndex, xIndex] = point.MeanValue;
            }
        }

        // Исправление Ambiguous invocation: считываем максимум прямо из списка
        _maxZ = matrixPoints.Max(p => p.MeanValue);
        if (_maxZ <= 0) _maxZ = 1.0;

        var mesh = new MeshGeometry3D();

        for (int y = 0; y < yValues.Length; y++)
        {
            double mVal = yValues[y];
            double normY = (_maxM > _minM) ? (mVal - _minM) / (_maxM - _minM) * BoxLengthM : 0;

            for (int x = 0; x < xValues.Length; x++)
            {
                double nVal = xValues[x];
                double normX = (_maxN > _minN) ? (nVal - _minN) / (_maxN - _minN) * BoxWidthN : 0;

                double rawZ = values[y, x];
                double normZ = (rawZ / _maxZ) * BoxHeightT;

                mesh.Positions.Add(new Point3D(normX, normY, normZ));

                double textureU = Math.Clamp(rawZ / _maxZ, 0.0, 1.0);
                mesh.TextureCoordinates.Add(new Point(textureU, 0.5));
            }
        }

        for (int y = 0; y < yValues.Length - 1; y++)
        {
            for (int x = 0; x < xValues.Length - 1; x++)
            {
                int i00 = y * xValues.Length + x;
                int i10 = y * xValues.Length + (x + 1);
                int i01 = (y + 1) * xValues.Length + x;
                int i11 = (y + 1) * xValues.Length + (x + 1);

                mesh.TriangleIndices.Add(i00);
                mesh.TriangleIndices.Add(i10);
                mesh.TriangleIndices.Add(i11);

                mesh.TriangleIndices.Add(i00);
                mesh.TriangleIndices.Add(i11);
                mesh.TriangleIndices.Add(i01);
            }
        }

        MatrixSurface.MeshGeometry = mesh;

        // Исправление CreateSpectrumMaterial: создание спектрального градиента вручную
        Material spectrumMaterial = CreateSpectrumMaterial();
        MatrixSurface.Material = spectrumMaterial;
        MatrixSurface.BackMaterial = spectrumMaterial;

        BuildAxesAndLabels(_minN, _maxN, _minM, _maxM, _maxZ);

        Matrix3DViewport.ZoomExtents();
    }

    private static Material CreateSpectrumMaterial()
    {
        var gradientBrush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 0)
        };

        gradientBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.0));
        gradientBrush.GradientStops.Add(new GradientStop(Colors.Cyan, 0.25));
        gradientBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.5));
        gradientBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.75));
        gradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));

        return new DiffuseMaterial(gradientBrush);
    }

    private void BuildAxesAndLabels(double minN, double maxN, double minM, double maxM, double maxZ)
    {
        var gridFloor = new GridLinesVisual3D
        {
            Width = BoxWidthN,
            Length = BoxLengthM,
            Center = new Point3D(BoxWidthN / 2.0, BoxLengthM / 2.0, 0),
            MinorDistance = 1.0,
            MajorDistance = 2.0,
            Thickness = 0.015,
            Fill = Brushes.DimGray
        };
        AxesContainer.Children.Add(gridFloor);

        AxesContainer.Children.Add(new ArrowVisual3D
        {
            Point1 = new Point3D(0, 0, 0),
            Point2 = new Point3D(BoxWidthN + 1.2, 0, 0),
            Diameter = 0.08,
            Fill = Brushes.DodgerBlue
        });

        AxesContainer.Children.Add(new ArrowVisual3D
        {
            Point1 = new Point3D(0, 0, 0),
            Point2 = new Point3D(0, BoxLengthM + 1.2, 0),
            Diameter = 0.08,
            Fill = Brushes.LimeGreen
        });

        AxesContainer.Children.Add(new ArrowVisual3D
        {
            Point1 = new Point3D(0, 0, 0),
            Point2 = new Point3D(0, 0, BoxHeightT + 1.0),
            Diameter = 0.08,
            Fill = Brushes.OrangeRed
        });

        AxesContainer.Children.Add(new BillboardTextVisual3D
        {
            Text = "Ось N (столбцы A)",
            Position = new Point3D(BoxWidthN / 2.0, -1.2, 0),
            FontSize = 16,
            Foreground = Brushes.DodgerBlue
        });

        AxesContainer.Children.Add(new BillboardTextVisual3D
        {
            Text = "Ось M (строки A)",
            Position = new Point3D(-1.2, BoxLengthM / 2.0, 0),
            FontSize = 16,
            Foreground = Brushes.LimeGreen
        });

        AxesContainer.Children.Add(new BillboardTextVisual3D
        {
            Text = "Ось T (мс)",
            Position = new Point3D(-0.8, -0.8, BoxHeightT + 1.3),
            FontSize = 16,
            Foreground = Brushes.OrangeRed
        });

        int steps = 5;
        for (int i = 0; i <= steps; i++)
        {
            double fraction = (double)i / steps;
            double posX = fraction * BoxWidthN;
            double valN = minN + fraction * (maxN - minN);

            AxesContainer.Children.Add(new BillboardTextVisual3D
            {
                Text = $"{valN:F0}",
                Position = new Point3D(posX, -0.4, 0),
                FontSize = 12,
                Foreground = Brushes.LightGray
            });
        }

        for (int i = 0; i <= steps; i++)
        {
            double fraction = (double)i / steps;
            double posY = fraction * BoxLengthM;
            double valM = minM + fraction * (maxM - minM);

            AxesContainer.Children.Add(new BillboardTextVisual3D
            {
                Text = $"{valM:F0}",
                Position = new Point3D(-0.4, posY, 0),
                FontSize = 12,
                Foreground = Brushes.LightGray
            });
        }

        for (int i = 0; i <= steps; i++)
        {
            double fraction = (double)i / steps;
            double posZ = fraction * BoxHeightT;
            double valT = fraction * maxZ;

            AxesContainer.Children.Add(new BillboardTextVisual3D
            {
                Text = $"{valT:F2}",
                Position = new Point3D(-0.4, -0.4, posZ),
                FontSize = 12,
                Foreground = Brushes.LightCoral
            });
        }
    }

    private void Matrix3DViewport_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed ||
            e.RightButton == MouseButtonState.Pressed ||
            e.MiddleButton == MouseButtonState.Pressed)
        {
            return;
        }

        if (TryHitTest(e.GetPosition(Matrix3DViewport), out double realN, out double realM, out double realT))
        {
            if (DataContext is MainWindowViewModel vm)
            {
                vm.Status = $"Курсор ➔ N: {realN:F0} | M: {realM:F0} | Время T: {realT:F4} мс";
            }
        }
    }

    // Поле для хранения текущего открытого ToolTip
    private ToolTip? _activeToolTip;

    private void Matrix3DViewport_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (TryHitTest(e.GetPosition(Matrix3DViewport), out double realN, out double realM, out double realT))
        {
            string info = $"Точка на 3D графике:\nN: {realN:F0}\nM: {realM:F0}\nВремя T: {realT:F4} мс";

            // Если предыдущая всплывающая подсказка еще открыта — закрываем её
            CloseToolTip();

            // Создаем ToolTip и привязываем его к позиции курсора
            _activeToolTip = new ToolTip
            {
                Content = info,
                PlacementTarget = Matrix3DViewport,
                Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                IsOpen = true
            };

            if (DataContext is MainWindowViewModel vm)
            {
                vm.Status = $"Точка фиксирована [N={realN:F0}, M={realM:F0}] -> {realT:F4} мс";
            }
        }
    }

// Отжатие кнопки мыши — закрываем плашку
    private void Matrix3DViewport_MouseUp(object sender, MouseButtonEventArgs e)
    {
        CloseToolTip();
    }

// Увод курсора за пределы viewport — закрываем плашку
    private void Matrix3DViewport_MouseLeave(object sender, MouseEventArgs e)
    {
        CloseToolTip();
    }

    private void CloseToolTip()
    {
        if (_activeToolTip != null)
        {
            _activeToolTip.IsOpen = false;
            _activeToolTip = null;
        }
    }

    private bool TryHitTest(Point mousePosition, out double realN, out double realM, out double realT)
    {
        realN = 0; realM = 0; realT = 0;
        RayMeshGeometry3DHitTestResult? rayHit = null;

        VisualTreeHelper.HitTest(
            Matrix3DViewport,
            null,
            result =>
            {
                if (result is RayMeshGeometry3DHitTestResult meshHit)
                {
                    rayHit = meshHit;
                    return HitTestResultBehavior.Stop;
                }
                return HitTestResultBehavior.Continue;
            },
            new PointHitTestParameters(mousePosition));

        if (rayHit is null) return false;

        Point3D p = rayHit.PointHit;

        realN = _minN + (p.X / BoxWidthN) * (_maxN - _minN);
        realM = _minM + (p.Y / BoxLengthM) * (_maxM - _minM);
        realT = (p.Z / BoxHeightT) * _maxZ;

        return true;
    }
}