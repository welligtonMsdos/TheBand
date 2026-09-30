using Microsoft.EntityFrameworkCore;
using TheBand.CoreDomain.Entities;

namespace TheBand.CoreInfrastructure.Data;

public sealed class CoreContext : DbContext
{
    public DbSet<Vinyl> Vinyls => Set<Vinyl>();

    public DbSet<Cassette> Cassettes => Set<Cassette>();

    public DbSet<Concert> Concerts => Set<Concert>();

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

        var cassette = modelBuilder.Entity<Cassette>();

        cassette.ToTable(nameof(Cassette));

        cassette.HasKey(item => item.Guid);

        cassette.Property(item => item.Artist).HasMaxLength(50).IsRequired();

        cassette.Property(item => item.Album).HasMaxLength(50).IsRequired();

        cassette.Property(item => item.Photo).HasMaxLength(255).IsRequired();

        cassette.Property(item => item.Price).HasPrecision(10, 2);

        cassette.Property(item => item.UserId).HasMaxLength(50).IsRequired();

        var concert = modelBuilder.Entity<Concert>();

        concert.ToTable(nameof(Concert));

        concert.HasKey(item => item.Guid);

        concert.Property(item => item.Artist).HasMaxLength(50).IsRequired();

        concert.Property(item => item.Venue).HasMaxLength(50).IsRequired();

        concert.Property(item => item.ShowDate).HasColumnType("date").IsRequired();

        concert.Property(item => item.Photo).HasMaxLength(255).IsRequired();

        concert.Property(item => item.UserId).HasMaxLength(50).IsRequired();
    }
}
