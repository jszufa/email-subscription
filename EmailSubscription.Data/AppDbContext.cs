using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmailSubscription.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    private string DbPath { get; set; }

    public AppDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Combine(path, "EmailSubscriptionApp.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}")
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>().HasData(
            new Group
            {
                Id = 1,
                Name = "Grupa Różowa"
            },
            new Group
            {
                Id = 2,
                Name = "Grupa Zielona"
            },
            new Group
            {
                Id = 3,
                Name = "Grupa Niebieska"
            }

        );
        
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Email, u.Name })
            .IsUnique();
        
        modelBuilder.Entity<Group>()
            .HasIndex(g => g.Name)
            .IsUnique();
    }
}