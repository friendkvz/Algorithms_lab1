namespace Algorithms_programm.Algorithms.Abstractions;

/// <summary>
/// Алгоритм, для которого в методичке измеряется КОЛИЧЕСТВО ОПЕРАЦИЙ (умножений),
/// а не время — используется для Части IV (возведение в степень x^n).
/// Метод возвращает и результат вычисления (нужен для юнит-тестов корректности),
/// и число шагов (нужно для графика "шаги от n").
/// </summary>
/// <typeparam name="TInput">Тип входных данных (например, запись (double X, int N)).</typeparam>
/// <typeparam name="TResult">Тип результата вычисления (double для x^n).</typeparam>
public interface IStepCountingAlgorithm<in TInput, TResult> : IAlgorithm
{
    (TResult Result, long Steps) ExecuteCountingSteps(TInput input);
}
