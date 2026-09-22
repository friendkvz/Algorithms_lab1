using Algorithms_programm.Algorithms.CustomAlgorithms;
using Algorithms_programm.Algorithms.MatrixOperations;
using Algorithms_programm.Algorithms.PowerAlgorithms;
using Algorithms_programm.Algorithms.VectorOperations;
using Algorithms_programm.DataGeneration;

namespace Algorithms_programm.Services;

/// <summary>
/// Реестр всех алгоритмов методички: связывает каждый алгоритм с его категорией сложности,
/// дефолтным N_max (пользователь может переопределить его в GUI) и генератором входных данных.
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
            new TimedAlgorithmRunner<MatrixPair, double[]>(
                new MatrixMultiplicationAlgorithm(), AlgorithmCategory.Cubic, 400,
                (n, m) =>
                {
                    var cols = m ?? n;
                    return new MatrixPair(matrixGenerator.Generate(n, cols), matrixGenerator.Generate(cols, n));
                },
                isMatrix: true),

            // ---- Часть III: единственный пользовательский алгоритм ----
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

    /// <summary>Основание x генерируется в [1.0, 2.0), чтобы избежать переполнения double при n до 1000.</summary>
    private PowerInput RandomPowerInput(int n) => new(1.0 + _random.NextDouble(), n);
}
