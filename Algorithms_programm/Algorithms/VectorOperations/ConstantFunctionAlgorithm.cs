using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.VectorOperations;

/// <summary>
/// Алгоритм 1 методички: постоянная функция f(v) = 1. Теоретическая сложность O(1).
/// Используется как "нулевой" алгоритм для калибровки и проверки, что даже минимальная
/// нагрузка на измерения (сохранение в БД, накладные расходы бенчмарка) не искажает картину.
/// </summary>
public sealed class ConstantFunctionAlgorithm : ITimedAlgorithm<double[], int>
{
    public string Name => "ConstantFunction";

    public int Execute(double[] input) => 1;
}
