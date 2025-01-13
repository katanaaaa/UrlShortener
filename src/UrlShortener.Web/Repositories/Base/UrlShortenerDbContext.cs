using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Repositories.Constants;
using UrlShortener.Web.Repositories.Models;

namespace UrlShortener.Web.Repositories.Base;

public class UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : DbContext(options)
{
    public DbSet<Url> Urls { get; set; }

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