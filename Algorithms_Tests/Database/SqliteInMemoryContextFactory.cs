using Algorithms_programm.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Algorithms_Tests.Database;

/// <summary>
/// Тестовая фабрика контекстов поверх SQLite ":memory:". У in-memory SQLite данные живут
/// только пока открыто конкретное соединение, поэтому соединение открывается один раз
/// в конструкторе и держится открытым на весь тест (Dispose закрывает его и уничтожает данные) —
/// это стандартный паттерн тестирования EfExperimentRepository/DbBackedCacheService без
/// реального файла БД и без применения миграций (используется EnsureCreated по текущей модели).
/// </summary>
public sealed class SqliteInMemoryContextFactory : IDbContextFactory<AppDbContext>, IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _options;

    public SqliteInMemoryContextFactory()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new AppDbContext(_options);
        context.Database.EnsureCreated();
    }

    public AppDbContext CreateDbContext() => new(_options);

    public Task<AppDbContext> CreateDbContextAsync(CancellationToken ct = default) =>
        Task.FromResult(CreateDbContext());

    public void Dispose() => _connection.Dispose();
}
