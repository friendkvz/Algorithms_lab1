using System;
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
    public IServiceProvider ServiceProvider { get; }

    public App()
    {
        var services = new ServiceCollection();

        services.AddDbContextFactory<Algorithms_programm.Database.AppDbContext>(
            options => options.UseSqlite(DbPathResolver.GetConnectionString()));

        services.AddSingleton<IExperimentRepository, EfExperimentRepository>();
        services.AddSingleton<ICacheService, DbBackedCacheService>();
        services.AddSingleton<AlgorithmRegistry>();
        services.AddSingleton<ExperimentOrchestrator>();
        services.AddSingleton<ComparisonService>();
        services.AddTransient<MainWindowViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        ApplyMigrations();
    }

    private void ApplyMigrations()
    {
        using var scope = ServiceProvider.CreateScope();

        var factory =
            scope.ServiceProvider
                .GetRequiredService<IDbContextFactory<Algorithms_programm.Database.AppDbContext>>();

        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = new MainWindow
        {
            DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>()
        };

        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        base.OnExit(e);
    }
}