using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestingMocks.UserApi.Configuration;
using TestingMocks.UserApi.Data;
using TestingMocks.UserApi.Services;
using TestingMocks.Models;
using TestingMocks.UserApi.Exceptions;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace TestingMocks.UserApiTests;

public class Tests
{
    private UserDbContext MakeInMemoryDb()
    {
        return new UserDbContext(new DbContextOptionsBuilder<UserDbContext>().UseInMemoryDatabase("test-db").Options);
    }
    private (UserDbContext db, IOptions<AuthConfiguration> options, PasswordHasherService hasher)
        BuildEnvironment()
    {
        var appSettings = new AuthConfiguration { PasswordSalt = "F0289917673B83D441DC03EA5D1C0345", Jwt = new() { SigningKey = "981461105F28A48CD2301F39B8659337", Expiration = TimeSpan.Parse("1.00:00:00") } };

        var appSettingsOptions = Options.Create(appSettings);
        var hasher = new PasswordHasherService(appSettingsOptions);
        var db = MakeInMemoryDb();

        return (db, appSettingsOptions, hasher);
    }

    [Test]
    [Arguments("test", "W7wpe62J6TJAa3D18uRj3L2uEeirIbdI3MjkKv7ORl8=")]
    [Arguments("", "Ym55mEJtdVCPZt6tMgeqPCcwzK7GMqO20dXCzaOca0E=")]
    public async Task PasswordHasherServiceTest(string password, string passwordHash)
    {
        var env = BuildEnvironment();

        await Assert.That(env.hasher.HashPassword(password)).IsEqualTo(passwordHash);
    }

    [Test]
    [Arguments("test", "test")]
    [Arguments("test3", "test3")]
    public async Task ShouldCreateValidUser(string username, string password)
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        await userService.RegisterAsync(username, password);

        await Assert.That(await env.db.Users.AnyAsync(u => u.Username == username && u.PasswordHash == env.hasher.HashPassword(password))).IsTrue();
    }

    [Test]
    [Arguments("te", "test")]
    [Arguments("testtesttesttesttesttesttesttest1", "test")]
    public async Task ShouldFailOnValidateUsernameOnRegister(string username, string password)
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        await Assert.ThrowsAsync<ValidationException>(async () => await userService.RegisterAsync(username, password));
    }

    [Test]
    [Arguments("test1", "test1")]
    public async Task ShouldUserLogin(string username, string password)
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        User[] addUsers = [new() { Username = "test1", PasswordHash = env.hasher.HashPassword("test1") }, new() { Username = "test2", PasswordHash = env.hasher.HashPassword("test2") }];
        env.db.Users.RemoveRange(env.db.Users);
        await env.db.Users.AddRangeAsync(addUsers);
        await env.db.SaveChangesAsync();

        var user = await userService.LoginAsync(username, password);

        await Assert.That(user.Username == username && user.PasswordHash == env.hasher.HashPassword(password)).IsTrue();
    }

    [Test]
    [Arguments("test1", "test11")]
    [Arguments("", "test2")]
    public async Task ShouldFailOnWrongUserNameOrPasswordOnLogin(string username, string password)
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        User[] addUsers = [new() { Username = "test1", PasswordHash = env.hasher.HashPassword("test1") }, new() { Username = "test2", PasswordHash = env.hasher.HashPassword("test2") }];
        env.db.Users.RemoveRange(env.db.Users);
        await env.db.Users.AddRangeAsync(addUsers);
        await env.db.SaveChangesAsync();


        await Assert.ThrowsAsync<BadRequestException>(async () => await userService.LoginAsync(username, password));
    }

    [Test]
    [Arguments("test1")]
    [Arguments("test2")]
    public async Task ShouldGetUser(string username)
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        User[] addUsers = [new() { Username = "test1", PasswordHash = env.hasher.HashPassword("test1") }, new() { Username = "test2", PasswordHash = env.hasher.HashPassword("test2") }];
        env.db.Users.RemoveRange(env.db.Users);
        await env.db.Users.AddRangeAsync(addUsers);
        await env.db.SaveChangesAsync();

        var user = await userService.GetUserAsync(username);

        await Assert.That(user.Username == username).IsTrue();
    }

    [Test]
    [Arguments("test22")]
    [Arguments("")]
    public async Task ShouldFailOnNotFoundUserOnGetUser(string username)
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        User[] addUsers = [new() { Username = "test1", PasswordHash = env.hasher.HashPassword("test1") }, new() { Username = "test2", PasswordHash = env.hasher.HashPassword("test2") }];
        env.db.Users.RemoveRange(env.db.Users);
        await env.db.Users.AddRangeAsync(addUsers);
        await env.db.SaveChangesAsync();

        await Assert.ThrowsAsync<NotFoundException>(async () => await userService.GetUserAsync(username));
    }

    [Test]
    public async Task ShouldGetAllUsers()
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        User[] addUsers = [new() { Username = "test1", PasswordHash = env.hasher.HashPassword("test1") }, new() { Username = "test2", PasswordHash = env.hasher.HashPassword("test2") }];
        env.db.Users.RemoveRange(env.db.Users);
        await env.db.Users.AddRangeAsync(addUsers);
        await env.db.SaveChangesAsync();

        var users = await userService.GetAllUsersAsync();

        await Assert.That(users.Count).IsEqualTo(2);
    }

    [Test]
    [Arguments("Name,Age,City\nJohn,30,New York", "John", 30, "New York")]
    [Arguments(" Name , Age , City \n John , 30 , New York ", "John", 30, "New York")]
    public async Task ShouldGetUserDetailsFromCSV(string csvStrig, string name, byte age, string city)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
        };
        using var csv = new CsvReader(new StringReader(csvStrig), config);
        var userDetails = await csv.GetRecordsAsync<UserDetails>().FirstOrDefaultAsync()
            ?? throw new BadRequestException("Malformed CSV.");

        await Assert.That(userDetails.Name == name).IsTrue();
    }

    [Test]
    [Arguments("test1", "Name,Age,City\nJohn,30,New York", "John", 30, "New York")]
    public async Task ShouldSetUserDetails(string username, string csvStrig, string name, byte age, string city)
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        User[] addUsers = [new() { Username = "test1", PasswordHash = env.hasher.HashPassword("test1") }, new() { Username = "test2", PasswordHash = env.hasher.HashPassword("test2") }];
        env.db.Users.RemoveRange(env.db.Users);
        await env.db.Users.AddRangeAsync(addUsers);
        await env.db.SaveChangesAsync();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
        };

        using var csv = new CsvReader(new StringReader(csvStrig), config);

        var userDetails = await csv.GetRecordsAsync<UserDetails>().FirstOrDefaultAsync()
            ?? throw new BadRequestException("Malformed CSV.");

        await userService.SetUserDetails(username, userDetails);

        userDetails = (await env.db.Users.FirstAsync(u => u.Username == username)).Details!;

        await Assert.That(userDetails.Name == name && userDetails.Age == age && userDetails.City == city).IsTrue();
    }


    [Test]
    [Arguments("test11", "Name,Age,City\nJohn,30,New York", "John", 30, "New York")]
    public async Task ShouldFailOnNotFoundUserOnSetUserDetails(string username, string csvStrig, string name, byte age, string city)
    {
        var env = BuildEnvironment();
        UserService userService = new(env.db, env.hasher);

        User[] addUsers = [new() { Username = "test1", PasswordHash = env.hasher.HashPassword("test1") }, new() { Username = "test2", PasswordHash = env.hasher.HashPassword("test2") }];
        env.db.Users.RemoveRange(env.db.Users);
        await env.db.Users.AddRangeAsync(addUsers);
        await env.db.SaveChangesAsync();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
        };

        using var csv = new CsvReader(new StringReader(csvStrig), config);

        var userDetails = await csv.GetRecordsAsync<UserDetails>().FirstOrDefaultAsync()
            ?? throw new BadRequestException("Malformed CSV.");

        await Assert.ThrowsAsync<NotFoundException>(async () => await userService.SetUserDetails(username, userDetails));
    }
}