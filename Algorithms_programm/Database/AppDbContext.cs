using Algorithms_programm.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Algorithms_programm.Database;

public class AppDbContext : DbContext
{
    public DbSet<AlgorithmEntity> Algorithms => Set<AlgorithmEntity>();
    public DbSet<ExperimentSessionEntity> ExperimentSessions => Set<ExperimentSessionEntity>();
    public DbSet<ExperimentRunEntity> ExperimentRuns => Set<ExperimentRunEntity>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AlgorithmEntity>(entity =>
        {
            entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
            entity.HasIndex(a => a.Name).IsUnique();
        });

        modelBuilder.Entity<ExperimentSessionEntity>(entity =>
        {
            entity.Property(s => s.ConfigHash).IsRequired().HasMaxLength(128);

            entity.HasOne(s => s.Algorithm)
                .WithMany(a => a.Sessions)
                .HasForeignKey(s => s.AlgorithmId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ускоряет поиск "есть ли уже сессия с такой конфигурацией у этого алгоритма" —
            // ключевой запрос для DbBackedCacheService.
            entity.HasIndex(s => new { s.AlgorithmId, s.ConfigHash });
        });

        modelBuilder.Entity<ExperimentRunEntity>(entity =>
        {
            entity.HasOne(r => r.Session)
                .WithMany(s => s.Runs)
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ускоряет выборку "все запуски для точки (n, m) в этой сессии" — основной запрос
            // и для кэша, и для построения графика эмпирических точек.
            entity.HasIndex(r => new { r.SessionId, r.N, r.M });
        });
    }
}
