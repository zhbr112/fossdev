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

        if (await db.Users.AnyAsync(u => u.Username == username)) throw new BadRequestException("Username taken", "username");

        var createdUser = await db.Users.AddAsync(user);

        await db.SaveChangesAsync();

        return createdUser.Entity;
    }
}