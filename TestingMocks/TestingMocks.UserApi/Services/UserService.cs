using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TestingMocks.Models;
using TestingMocks.UserApi.Data;
using TestingMocks.UserApi.Exceptions;

namespace TestingMocks.UserApi.Services;

/// <summary>
/// Сервис работы с API пользователей
/// </summary>
/// <param name="db">Используемая база данных</param>
/// <param name="hasher">Используемый сервис хэширования</param>
public class UserService(UserDbContext db, PasswordHasherService hasher)
{
    /// <summary>
    /// Зарегистрировать пользователя
    /// </summary>
    /// <param name="username">Имя пользователя (логин)</param>
    /// <param name="password">Пароль</param>
    /// <returns>Информация о пользователе</returns>
    /// <exception cref="BadRequestException">Пользователь с таким именем уже существует</exception>
    public async Task<User> RegisterAsync(string username, string password)
    {
        var user = new User
        {
            Username = username,
            PasswordHash = hasher.HashPassword(password)
        };

        Validator.ValidateObject(user, new(user), true);

        if (await db.Users.AnyAsync(u => u.Username == username)) throw new BadRequestException("Username taken", nameof(username));

        var createdUser = await db.Users.AddAsync(user);

        await db.SaveChangesAsync();

        return createdUser.Entity;
    }

    /// <summary>
    /// Войти в аккаунт
    /// </summary>
    /// <param name="username">Имя пользователя (логин)</param>
    /// <param name="password">Пароль</param>
    /// <returns>Информация о текущем пользователе и токен авторизации</returns>
    /// <exception cref="BadRequestException">Неверное имя пользователя или пароль</exception>
    public async Task<User> LoginAsync(string username, string password)
    {
        var user = await db.Users.SingleOrDefaultAsync(
            u => u.Username == username
            && u.PasswordHash == hasher.HashPassword(password))
            ?? throw new BadRequestException("Wrong username or password", nameof(username), nameof(password));

        return user;
    }

    /// <summary>
    /// Получить пользователя
    /// </summary>
    /// <param name="username">Имя пользователя (логин)</param>
    /// <returns>Информация о пользователе</returns>
    /// <exception cref="NotFoundException">Пользователь с таким именем не найден</exception>
    public async Task<User> GetUserAsync(string username)
    {
        var user = await db.Users
            .Include(u => u.Details)
            .FirstOrDefaultAsync(u => u.Username == username)
            ?? throw new NotFoundException("User not found");
        return user;
    }

    /// <summary>
    /// Получить список всех пользователей
    /// </summary>
    /// <returns>Список информации о пользователях</returns>
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await db.Users.Include(u => u.Details).ToListAsync();
    }

    /// <summary>
    /// Обновить информацию о текущем пользователе
    /// </summary>
    /// <param name="username">Имя пользователя (логин)</param>
    /// <param name="details">Информация о пользователе</param>
    /// <returns>Обновленный пользователь</returns>
    /// <exception cref="NotFoundException">Пользователь с таким именем не найден</exception>
    public async Task<User> SetUserDetails(string username, UserDetails details)
    {
        var user = await db.Users.FindAsync(username)
            ?? throw new NotFoundException("User not found");

        Validator.ValidateObject(details, new(details), true);

        user.Details = details;

        await db.SaveChangesAsync();

        return user;
    }
}