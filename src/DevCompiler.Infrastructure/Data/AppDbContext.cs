using DevCompiler.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevCompiler.Infrastructure.Data;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Room> Rooms => Set<Room>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

            entity.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(50000);

            entity.Property(e => e.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(100);

            entity.Property(e => e.CreatedByNickname)
            .IsRequired()
            .HasMaxLength(100);

            // We need to comma seperate because SQL Server is storing list that way
            entity.Property(e => e.Participants)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasMaxLength(1000);

            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.CreatedAt);

            // Simple Seed 
            var testRoom = Room.Create("Test Room", "tester", "System");
            entity.HasData(testRoom);
        });
    }
}