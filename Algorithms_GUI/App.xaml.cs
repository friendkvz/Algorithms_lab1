using System.Windows;
using Algorithms_programm.Caching;
using Algorithms_programm.Database;
using Algorithms_programm.Database.Repositories;
using Algorithms_programm.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Algorithms_GUI;

public partial class App : Application
{
    public IServiceProvider Services { get; }

    public App()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<Algorithms_programm.Database.AppDbContext>(options =>
            options.UseSqlite(DbPathResolver.GetConnectionString()));
        services.AddSingleton<IExperimentRepository, EfExperimentRepository>();
        services.AddSingleton<ICacheService, DbBackedCacheService>();
        services.AddSingleton<AlgorithmRegistry>();
        services.AddSingleton<ExperimentOrchestrator>();
        services.AddSingleton<ComparisonService>();
        services.AddSingleton<MainWindowViewModel>();
        Services = services.BuildServiceProvider();

        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IDbContextFactory<Algorithms_programm.Database.AppDbContext>>()
            .CreateDbContext().Database.Migrate();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() }.Show();
    }
}
