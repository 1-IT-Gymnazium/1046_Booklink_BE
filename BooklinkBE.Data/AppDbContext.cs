using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BooklinkBE.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Household> Households { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Bookshelf> Bookshelves { get; set; }
    public DbSet<EmailMessage> EmailMessages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<IdentityUserRole<Guid>>();
        modelBuilder.Ignore<IdentityRole<Guid>>();
        modelBuilder.Ignore<IdentityUserLogin<Guid>>();
        modelBuilder.Ignore<IdentityUserToken<Guid>>();
        modelBuilder.Ignore<IdentityRoleClaim<Guid>>();
        
        base.OnModelCreating(modelBuilder);

        // One-to-Many: User -> Households (Cascade on delete)
        modelBuilder.Entity<Household>()
            .HasOne(re => re.User)
            .WithMany(u => u.Households)
            .HasForeignKey(re => re.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many: Household -> Rooms (Cascade on delete)
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Household)
            .WithMany(re => re.Rooms)
            .HasForeignKey(r => r.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many: one Room has many BookShelves (Cascade on delete)
        modelBuilder.Entity<Bookshelf>()
            .HasOne(bs => bs.Room)
            .WithMany(r => r.Bookshelves)
            .HasForeignKey(bs => bs.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // One-to-Many: one BookShelf has many Books (Cascade on delete)
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Bookshelf)
            .WithMany(bs => bs.Books)
            .HasForeignKey(b => b.BookshelfId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}