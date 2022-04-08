using Microsoft.EntityFrameworkCore;
using SamuraiApp.Domain;

namespace SamuraiApp.Data;

public class SamuraiContext : DbContext
{
    public DbSet<Samurai> Samurais { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<Battle> Battles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Data Source=localhost,9001;Initial Catalog=SamuraiAppDb;Persist Security Info=True;User ID=sa;Password=Password01!;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Samurai>()
            // Define the many to many relationship
            .HasMany(s => s.Battles)
            .WithMany(b => b.Samurais)
            // Tell EF to use an entity for the relationships instead of an automatic one
            .UsingEntity<BattleSamurai>(
                bs => bs.HasOne<Battle>().WithMany(),
                bs => bs.HasOne<Samurai>().WithMany()
            )
            // Set the DateJoined property to be automatically generated by the database on insert
            .Property(bs => bs.DateJoined)
            .HasDefaultValueSql("GETDATE()");
    }
}