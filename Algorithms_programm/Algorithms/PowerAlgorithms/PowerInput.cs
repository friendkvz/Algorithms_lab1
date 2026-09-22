namespace Algorithms_programm.Algorithms.PowerAlgorithms;

/// <summary>
/// Вход для алгоритмов возведения в степень x^n (Часть IV методички, n от 1 до 1000).
/// X ограничен разумным диапазоном (например [1.0, 2.0]) во всех генераторах данных, чтобы
/// double не переполнялся до Infinity на больших n — умножения всё равно считаются честно,
/// но результат остаётся конечным и пригодным для проверки в тестах.
/// </summary>
/// <param name="X">Основание степени.</param>
/// <param name="N">Показатель степени, N >= 0.</param>
public readonly record struct PowerInput(double X, int N);
