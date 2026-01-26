using Microsoft.EntityFrameworkCore;
using SortFlow.Domain.Entities;

namespace SortFlow.Infrastructure.Data;

public class SortFlowDbContext : DbContext
{
    public SortFlowDbContext(DbContextOptions<SortFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<SortingEvent> SortingEvents => Set<SortingEvent>();
    public DbSet<SortingException> SortingExceptions => Set<SortingException>();
    public DbSet<SortingStation> SortingStations => Set<SortingStation>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<AppSettings> AppSettings => Set<AppSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(10).IsRequired();
        });

        modelBuilder.Entity<SortingStation>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.StationCode).HasMaxLength(20).IsRequired();
            entity.HasOne(x => x.Zone)
                .WithMany(z => z.SortingStations)
                .HasForeignKey(x => x.ZoneId);
        });

        modelBuilder.Entity<SortingEvent>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ItemId).HasMaxLength(50).IsRequired();
            entity.Property(x => x.PostalCode).HasMaxLength(12).IsRequired();
            entity.Property(x => x.Result).HasMaxLength(50).IsRequired();
            entity.HasOne(x => x.SortingStation)
                .WithMany(s => s.SortingEvents)
                .HasForeignKey(x => x.StationId);
            entity.HasOne(x => x.Zone)
                .WithMany(z => z.SortingEvents)
                .HasForeignKey(x => x.ZoneId);
            entity.HasOne(x => x.SortingException)
                .WithOne(e => e.SortingEvent)
                .HasForeignKey<SortingException>(e => e.SortingEventId);

            entity.HasIndex(x => x.Timestamp);
            entity.HasIndex(x => x.ZoneId);
            entity.HasIndex(x => x.StationId);
        });

        modelBuilder.Entity<SortingException>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Details).HasMaxLength(250).IsRequired();
            entity.HasIndex(x => x.ExceptionType);
            entity.HasIndex(x => x.Timestamp);
        });

        modelBuilder.Entity<AppSettings>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.EnableModules).HasMaxLength(500).IsRequired();
        });
    }
}
