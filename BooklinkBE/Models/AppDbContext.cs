using Microsoft.EntityFrameworkCore;

namespace Booklink.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasOne<User>(b => b.User)
            .WithMany(u => u.Books)
            .HasForeignKey(b => b.UserId);
    }
}
