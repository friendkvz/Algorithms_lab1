using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Algorithms_programm.Database;

/// <summary>
/// Нужен исключительно для design-time инструментов EF Core (dotnet-ef): когда вы выполняете
///   dotnet ef migrations add InitialCreate --project Algorithms_programm
/// EF Core не поднимает всё приложение (WPF Application не запускается), а создаёт контекст
/// через эту фабрику. Рантайм-код приложения её не использует — там AppDbContext
/// регистрируется через DI с той же строкой подключения (DbPathResolver.GetConnectionString()).
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(DbPathResolver.GetConnectionString());
        return new AppDbContext(optionsBuilder.Options);
    }
}
