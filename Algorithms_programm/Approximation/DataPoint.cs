namespace Algorithms_programm.Approximation;

/// <summary>
/// Одна эмпирическая точка для аппроксимации: n и измеренное значение (среднее время в мс
/// для Частей I–III, среднее число шагов для Части IV — с точки зрения этого слоя это просто
/// число T_empirical(n), откуда берётся значение, аппроксимации не важно).
/// </summary>
/// <param name="N">Размер входных данных (для матриц — обычно берётся n при фиксированном m, либо n*m — решает вызывающий код).</param>
/// <param name="Value">Измеренное среднее значение T_empirical(n).</param>
public readonly record struct DataPoint(double N, double Value);
