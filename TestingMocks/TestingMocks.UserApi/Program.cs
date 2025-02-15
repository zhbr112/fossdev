using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using TestingMocks.Models;
using TestingMocks.UserApi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<UserDbContext>
        (
            conf => { conf.UseInMemoryDatabase("tmpdb"); },
            ServiceLifetime.Singleton
        );
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/registration", async (UserDTO userDTO, UserDbContext db) =>
{
    string passwordHash;
    using (var sha256 = SHA256.Create())
    {
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(userDTO.Password));
        passwordHash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    User user = new(userDTO.Username, passwordHash);

    ValidationContext context = new(user);

    if (!Validator.TryValidateObject(user, context, null, true)) 
        return Results.BadRequest("Неверные данные");

    await db.Users.AddAsync(user);
    
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();

