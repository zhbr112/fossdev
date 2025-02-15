using Microsoft.EntityFrameworkCore;
using TestingMocks.Models;

namespace TestingMocks.UserApi.Data;

public class UserDbContext:DbContext
{
    DbSet<User> Users { get; set; }

    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        if (Database.IsInMemory())
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}
