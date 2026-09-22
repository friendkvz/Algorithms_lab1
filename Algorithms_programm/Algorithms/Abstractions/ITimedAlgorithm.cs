namespace Algorithms_programm.Algorithms.Abstractions;

/// <summary>
/// Алгоритм, для которого в методичке измеряется ВРЕМЯ выполнения (Части I, II, III).
/// Сам интерфейс не занимается замером времени — это делает TimeBenchmarkRunner
/// (Stopwatch вокруг вызова Execute). Такое разделение позволяет:
///  - переиспользовать Execute() в юнит-тестах для проверки корректности результата,
///  - не привязывать бизнес-логику алгоритма к деталям бенчмаркинга.
/// </summary>
/// <typeparam name="TInput">Тип входных данных алгоритма (например, double[] для векторных операций).</typeparam>
/// <typeparam name="TResult">Тип результата вычисления (например, double для суммы, double[] для сортировки).</typeparam>
public interface ITimedAlgorithm<in TInput, out TResult> : IAlgorithm
{
    TResult Execute(TInput input);
}
