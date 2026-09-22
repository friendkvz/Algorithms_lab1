namespace Algorithms_programm.Algorithms.Abstractions;

/// <summary>
/// Базовый маркерный интерфейс для всех алгоритмов лабораторной работы.
/// Name используется как ключ при сохранении результатов в БД и при кэшировании,
/// поэтому должен быть уникальным и стабильным (не менять между запусками).
/// </summary>
public interface IAlgorithm
{
    string Name { get; }
}
