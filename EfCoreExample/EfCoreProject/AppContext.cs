using Microsoft.EntityFrameworkCore;

namespace EfCoreProject;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100);
        });
    }

    public DbSet<Item> Items { get; set; }

}

public class Item
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
