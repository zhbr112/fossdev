using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TestingMocks.Models;
using TestingMocks.UserApi.Data;
using TestingMocks.UserApi.Exceptions;

namespace TestingMocks.UserApi.Services;

public class UserService(UserDbContext db, PasswordHasherService hasher)
{
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

    public async Task<User> LoginAsync(string username, string password)
    {
        var user = await db.Users.SingleOrDefaultAsync(
            u => u.Username == username
            && u.PasswordHash == hasher.HashPassword(password))
            ?? throw new BadRequestException("Wrong username or password", nameof(username), nameof(password));

        return user;
    }

    public async Task<User> GetUserAsync(string username)
    {
        var user = await db.Users.FindAsync(username)
            ?? throw new NotFoundException("User not found");
        return user;
    }
}