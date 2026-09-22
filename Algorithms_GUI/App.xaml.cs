using System;
using System.Windows;
using System.Windows.Threading;
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
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

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
            scope.ServiceProvider.GetRequiredService<
                IDbContextFactory<Algorithms_programm.Database.AppDbContext>>();

        using var context = factory.CreateDbContext();
        context.Database.Migrate();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var mainWindow = new MainWindow
            {
                DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>()
            };

            MainWindow = mainWindow;
            mainWindow.Show();
        }
        catch (Exception exception)
        {
            ShowFatalError(exception);
            Shutdown(-1);
        }
    }

    private void OnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        ShowFatalError(e.Exception);
        e.Handled = true;
    }

    private static void OnUnhandledException(
        object sender,
        UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            MessageBox.Show(
                exception.ToString(),
                "Критическая ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static void ShowFatalError(Exception exception)
    {
        MessageBox.Show(
            exception.ToString(),
            "Ошибка приложения",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
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