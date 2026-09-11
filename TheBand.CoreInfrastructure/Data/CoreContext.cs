using Microsoft.EntityFrameworkCore;
using TheBand.CoreDomain.Entities;

namespace TheBand.CoreInfrastructure.Data;

public sealed class CoreContext : DbContext
{
    public DbSet<Vinyl> Vinyls => Set<Vinyl>();

    public CoreContext(DbContextOptions<CoreContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var vinyl = modelBuilder.Entity<Vinyl>();

        vinyl.ToTable(nameof(Vinyl));

        vinyl.HasKey(item => item.Guid);

        vinyl.Property(item => item.Artist).HasMaxLength(50).IsRequired();

        vinyl.Property(item => item.Album).HasMaxLength(50).IsRequired();

        vinyl.Property(item => item.Photo).HasMaxLength(255).IsRequired();

        vinyl.Property(item => item.Price).HasPrecision(10, 2);

        vinyl.Property(item => item.UserId).HasMaxLength(50).IsRequired();
    }
}
