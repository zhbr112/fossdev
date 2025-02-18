using Microsoft.EntityFrameworkCore;
using TestingMocks.Models;

namespace TestingMocks.UserApi.Data;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserDetails> UserDetails { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        if (Database.IsInMemory())
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().OwnsOne(u => u.Details);
    }
}
