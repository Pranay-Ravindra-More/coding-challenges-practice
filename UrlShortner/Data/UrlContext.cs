using Microsoft.EntityFrameworkCore;
using UrlShortner.Model;

namespace UrlShortner.Data
{
    public class UrlContext:DbContext
    {
        public UrlContext(DbContextOptions<UrlContext> options) : base(options)
        {
        }

        public DbSet<UrlRedirectInfo> UrlRedirects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UrlRedirectInfo>()
                .HasIndex(x => x.hashKey)
                .IsUnique();
        }
    }
}
