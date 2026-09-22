using Algorithms_programm.Algorithms.CustomAlgorithms;
using Algorithms_programm.Algorithms.MatrixOperations;
using Algorithms_programm.Algorithms.PowerAlgorithms;
using Algorithms_programm.Algorithms.VectorOperations;
using Algorithms_programm.DataGeneration;

namespace Algorithms_programm.Services;

/// <summary>
/// Реестр всех алгоритмов методички: связывает каждый алгоритм с его категорией сложности,
/// дефолтным N_max ("не хардкодить N_max... дефолтные значения задавать константами по
/// категориям сложности, но всегда давать пользователю их переопределить в GUI") и генератором
/// входных данных. GUI берёт список Name/Category/DefaultNMax отсюда для выпадающего списка,
/// пользователь может переопределить N_max — сам реестр этого не хранит, это уже параметр
/// конкретного запуска (ExperimentConfig).
///
/// Дефолты N_max подобраны по правилу методички: при n = N_max время должно быть 5–10 сек и не
/// микросекунды. Это ориентировочные величины для типичного ПК — пользователь может (и должен
/// при необходимости) скорректировать их в GUI под свою машину.
/// </summary>
public sealed class AlgorithmRegistry
{
    private readonly Dictionary<string, IAlgorithmRunner> _runners;
    private readonly Random _random = new();

    public AlgorithmRegistry()
    {
        var vectorGenerator = new VectorGenerator();
        var integerVectorGenerator = new IntegerVectorGenerator();
        var matrixGenerator = new MatrixGenerator();

        var list = new List<IAlgorithmRunner>
        {
            // ---- Часть I: операции с вектором (алгоритмы 1-7) ----
            new TimedAlgorithmRunner<double[], int>(
                new ConstantFunctionAlgorithm(), AlgorithmCategory.Constant, 2_000_000,
                (n, _) => vectorGenerator.Generate(n)),

            new TimedAlgorithmRunner<double[], double>(
                new SumAlgorithm(), AlgorithmCategory.Linear, 500_000,
                (n, _) => vectorGenerator.Generate(n)),

            new TimedAlgorithmRunner<double[], double>(
                new ProductAlgorithm(), AlgorithmCategory.Linear, 500_000,
                (n, _) => vectorGenerator.Generate(n)),

            new TimedAlgorithmRunner<double[], double>(
                new PolynomialNaiveAlgorithm(), AlgorithmCategory.Linear, 200_000,
                (n, _) => vectorGenerator.Generate(n)),

            new TimedAlgorithmRunner<double[], double>(
                new PolynomialHornerAlgorithm(), AlgorithmCategory.Linear, 500_000,
                (n, _) => vectorGenerator.Generate(n)),

            new TimedAlgorithmRunner<double[], double[]>(
                new BubbleSortAlgorithm(), AlgorithmCategory.Quadratic, 30_000,
                (n, _) => vectorGenerator.Generate(n)),

            new TimedAlgorithmRunner<double[], double[]>(
                new QuickSortAlgorithm(), AlgorithmCategory.LinearLogarithmic, 2_000_000,
                (n, _) => vectorGenerator.Generate(n)),

            new TimedAlgorithmRunner<double[], double[]>(
                new BuiltInSortAlgorithm(), AlgorithmCategory.LinearLogarithmic, 2_000_000,
                (n, _) => vectorGenerator.Generate(n)),

            // ---- Часть II: матрицы (алгоритм 8) ----
            new TimedAlgorithmRunner<MatrixPair, double[,]>(
                new MatrixMultiplicationAlgorithm(), AlgorithmCategory.Cubic, 400,
                (n, m) =>
                {
                    var cols = m ?? n; // если m не задан явно, считаем квадратный случай n == m
                    return new MatrixPair(matrixGenerator.Generate(n, cols), matrixGenerator.Generate(cols, n));
                },
                isMatrix: true),

            // ---- Часть III: индивидуальное задание (custom, точка расширения) ----
            new TimedAlgorithmRunner<int[], int[]>(
                new HanSortAlgorithm(), AlgorithmCategory.Linear, 1_000_000,
                (n, _) => integerVectorGenerator.Generate(n)),

            // ---- Часть IV: возведение в степень (алгоритмы 10-12), измеряются шаги ----
            new StepCountingAlgorithmRunner<PowerInput, double>(
                new IterativePowerAlgorithm(), 1000, n => RandomPowerInput(n)),

            new StepCountingAlgorithmRunner<PowerInput, double>(
                new RecursivePowerAlgorithm(), 1000, n => RandomPowerInput(n)),

            new StepCountingAlgorithmRunner<PowerInput, double>(
                new FastPowerAlgorithm(), 1000, n => RandomPowerInput(n)),
        };

        _runners = list.ToDictionary(r => r.Name, StringComparer.Ordinal);
    }

    public IReadOnlyList<IAlgorithmRunner> All => _runners.Values.ToList();

    public IAlgorithmRunner Get(string algorithmName)
    {
        if (!_runners.TryGetValue(algorithmName, out var runner))
        {
            throw new KeyNotFoundException(
                $"Алгоритм '{algorithmName}' не зарегистрирован. Доступные: {string.Join(", ", _runners.Keys)}.");
        }

        return runner;
    }

    /// <summary>Основание x генерируется в [1.0, 2.0) — избегает переполнения double даже при n до 1000.</summary>
    private PowerInput RandomPowerInput(int n) => new(1.0 + _random.NextDouble(), n);
}
