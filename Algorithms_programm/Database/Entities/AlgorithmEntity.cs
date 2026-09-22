namespace Algorithms_programm.Database.Entities;

/// <summary>
/// Справочник алгоритмов. Name соответствует IAlgorithm.Name (например "QuickSort",
/// "MatrixMultiplication", "FastPower") и используется как стабильный ключ между запусками
/// приложения — сами C#-классы алгоритмов в БД не хранятся, только их имя.
/// </summary>
public class AlgorithmEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<ExperimentSessionEntity> Sessions { get; set; } = new List<ExperimentSessionEntity>();
}
