namespace Algorithms_programm.Database;

/// <summary>
/// Находит корень решения (папку с AlgorithmComplexityAnalysis.sln) и путь к файлу БД
/// Data/experiments.db в корне решения — а не в bin/Debug/net10.0, куда попадает
/// AppContext.BaseDirectory при запуске из Rider/dotnet run.
/// </summary>
public static class DbPathResolver
{
    private const string SolutionFileName = "AlgorithmComplexityAnalysis.sln";
    private const string DataFolderName = "Data";
    private const string DatabaseFileName = "experiments.db";

    /// <summary>
    /// Полный путь к файлу БД в Data/ в корне решения. Папка Data создаётся, если её ещё нет.
    /// </summary>
    public static string GetDatabaseFilePath()
    {
        var solutionRoot = FindSolutionRoot(AppContext.BaseDirectory);
        var dataFolder = Path.Combine(solutionRoot, DataFolderName);
        Directory.CreateDirectory(dataFolder);
        return Path.Combine(dataFolder, DatabaseFileName);
    }

    public static string GetConnectionString() => $"Data Source={GetDatabaseFilePath()}";

    /// <summary>
    /// Поднимается по дереву каталогов вверх от startDirectory в поисках .sln-файла.
    /// Если по какой-то причине .sln не найден (например, БД используется вне контекста
    /// решения — в тестах CI), возвращает startDirectory как разумный fallback.
    /// </summary>
    private static string FindSolutionRoot(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            if (directory.GetFiles(SolutionFileName).Length > 0 ||
                directory.GetFiles("*.sln").Length > 0)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return startDirectory;
    }
}
