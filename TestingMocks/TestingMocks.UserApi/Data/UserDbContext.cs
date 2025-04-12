using Microsoft.EntityFrameworkCore;
using TestingMocks.Models;

namespace TestingMocks.UserApi.Data;

/// <summary>
/// Контекст базы данных пользователей
/// </summary>
public class UserDbContext : DbContext
{
    /// <summary>
    /// Таблица пользователей
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Таблица информации пользователей
    /// </summary>
    public DbSet<UserDetails> UserDetails { get; set; }

    /// <summary>
    /// Создание контекста базы данных пользователей
    /// </summary>
    /// <param name="options">Конфигурация базы данных</param>
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
        if (Database.IsInMemory())
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }

    /// <summary>
    /// Конфигурация моделей
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().OwnsOne(u => u.Details);
    }
}
