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
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            entity.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(a => a.Name)
                .IsUnique();
        });

        modelBuilder.Entity<ExperimentSessionEntity>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            entity.Property(s => s.ConfigHash)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasOne(s => s.Algorithm)
                .WithMany(a => a.Sessions)
                .HasForeignKey(s => s.AlgorithmId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => new
            {
                s.AlgorithmId,
                s.ConfigHash
            });
        });

        modelBuilder.Entity<ExperimentRunEntity>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(r => r.Session)
                .WithMany(s => s.Runs)
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => new
            {
                r.SessionId,
                r.N,
                r.M
            });
        });
    }
}
