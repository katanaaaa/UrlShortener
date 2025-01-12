using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Repositories.Constants;
using UrlShortener.Web.Repositories.Models;

namespace UrlShortener.Web.Repositories.Base;

public class UrlShortenerDbContext : DbContext
{
    public DbSet<Url> Urls { get; set; }

    public string DbPath { get; }

    public UrlShortenerDbContext()
    {
        DbPath = Path.Join("./", "app.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Url>(builder =>
        {
            builder
                .HasKey(url => url.Id);
            
            builder
                .Property(url => url.Code)
                .HasMaxLength(ShortLinkSettings.Length);
            
            builder
                .HasIndex(url => url.Code)
                .IsUnique();
        });
    }
}