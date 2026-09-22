using Algorithms_programm.Algorithms.Abstractions;
using Algorithms_programm.Algorithms.MatrixOperations;
using Algorithms_programm.Algorithms.PowerAlgorithms;
using Algorithms_programm.Algorithms.VectorOperations;
using Algorithms_programm.DataGeneration;

namespace Algorithms_programm.Services;

public sealed class AlgorithmRegistry
{
    private readonly Dictionary<string, IAlgorithmRunner> _runners;
    private readonly Random _random = new();
    public AlgorithmRegistry()
    {
        var vectors = new VectorGenerator(); var integers = new IntegerVectorGenerator(); var matrices = new MatrixGenerator();
        var list = new List<IAlgorithmRunner>
        {
            new TimedAlgorithmRunner<double[], int>(new ConstantFunctionAlgorithm(), AlgorithmCategory.Constant, 2_000_000, (n, _) => vectors.Generate(n)),
            new TimedAlgorithmRunner<double[], double>(new SumAlgorithm(), AlgorithmCategory.Linear, 500_000, (n, _) => vectors.Generate(n)),
            new TimedAlgorithmRunner<double[], double>(new ProductAlgorithm(), AlgorithmCategory.Linear, 500_000, (n, _) => vectors.Generate(n)),
            new TimedAlgorithmRunner<double[], double>(new PolynomialNaiveAlgorithm(), AlgorithmCategory.Linear, 200_000, (n, _) => vectors.Generate(n)),
            new TimedAlgorithmRunner<double[], double>(new PolynomialHornerAlgorithm(), AlgorithmCategory.Linear, 500_000, (n, _) => vectors.Generate(n)),
            new TimedAlgorithmRunner<double[], double[]>(new BubbleSortAlgorithm(), AlgorithmCategory.Quadratic, 30_000, (n, _) => vectors.Generate(n)),
            new TimedAlgorithmRunner<double[], double[]>(new QuickSortAlgorithm(), AlgorithmCategory.LinearLogarithmic, 2_000_000, (n, _) => vectors.Generate(n)),
            new TimedAlgorithmRunner<double[], double[]>(new BuiltInSortAlgorithm(), AlgorithmCategory.LinearLogarithmic, 2_000_000, (n, _) => vectors.Generate(n)),
            new TimedAlgorithmRunner<MatrixPair, double[,]>(new MatrixMultiplicationAlgorithm(), AlgorithmCategory.Cubic, 400, (n, m) => { var cols = m ?? n; return new MatrixPair(matrices.Generate(n, cols), matrices.Generate(cols, n)); }, isMatrix: true),
            new TimedAlgorithmRunner<int[], int[]>(new HanSortAlgorithm(), AlgorithmCategory.Linear, 1_000_000, (n, _) => integers.Generate(n)),
            new StepCountingAlgorithmRunner<PowerInput, double>(new IterativePowerAlgorithm(), 1000, n => RandomPowerInput(n)),
            new StepCountingAlgorithmRunner<PowerInput, double>(new RecursivePowerAlgorithm(), 1000, n => RandomPowerInput(n)),
            new StepCountingAlgorithmRunner<PowerInput, double>(new FastPowerAlgorithm(), 1000, n => RandomPowerInput(n))
        };
        _runners = list.ToDictionary(r => r.Name, StringComparer.Ordinal);
    }
    public IReadOnlyList<IAlgorithmRunner> All => _runners.Values.ToList();
    public IAlgorithmRunner Get(string name) => _runners.TryGetValue(name, out var r) ? r : throw new KeyNotFoundException($"Алгоритм '{name}' не зарегистрирован.");
    private PowerInput RandomPowerInput(int n) => new(1.0 + _random.NextDouble(), n);
}
