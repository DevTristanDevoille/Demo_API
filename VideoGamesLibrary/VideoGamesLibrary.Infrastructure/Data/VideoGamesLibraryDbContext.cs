using Microsoft.EntityFrameworkCore;
using VideoGamesLibrary.Domain.Entities;

namespace VideoGamesLibrary.Infrastructure.Data;

public class VideoGameLibraryDbContext : DbContext
{
    public VideoGameLibraryDbContext(DbContextOptions<VideoGameLibraryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Exemple de configuration fluide
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Games");

            entity.HasKey(g => g.Id);

            entity.Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(g => g.Platform)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(g => g.Genre)
                .HasMaxLength(100);
        });
    }
}
