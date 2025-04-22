using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Elastic.CommonSchema.Serilog;
using ElkTest.Models;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Formatting.Json;

List<User> users = [];

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(new EcsTextFormatter(), "logs/app.log")
    // .WriteTo.Http(
    //     builder.Configuration.GetValue<string>("LogstashUrl")
    //         ?? throw new KeyNotFoundException("No Logstash URL provided."),
    //     queueLimitBytes: null)
    .CreateLogger();

builder.Services.AddOpenApi();
builder.Services.AddSerilog();
//builder.Services.AddHttpLogging();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    //app.UseHttpLogging();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapPost("/user/register", (string username, string password) =>
{
    var dto = new CreateUserDTO
    {
        Username = username,
        Password = password
    };

    if (users.Any(x => x.Username == dto.Username))
    {
        app.Logger.LogError("Register request denied - username {username} is already taken.", dto.Username);

        return Results.BadRequest("Username taken.");
    }

    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(dto.Password));

    app.Logger.LogDebug("User registration request:\nUsername: {username}\nPassword hash: {hash}", dto.Username, hash);

    List<ValidationResult> errors = [];
    if (!Validator.TryValidateObject(dto, new(dto), errors, true))
    {
        app.Logger.LogError("Validation error while registering user {username}:\n{error}",
            dto.Username,
            string.Join(
                "\n.",
                errors.Select(
                    x => $"{string.Join(", ", x.MemberNames)} - {x.ErrorMessage}")
                    ));
        return Results.ValidationProblem(errors.ToDictionary(x => x.ErrorMessage ?? "...", x => x.MemberNames.ToArray()));
    }

    var user = new User
    {
        Username = dto.Username,
        PasswordHash = Encoding.UTF8.GetString(hash)
    };

    users.Add(user);

    app.Logger.LogInformation("User {username} ({id}) was registered.", user.Username, user.Id);

    return Results.Ok((UserDTO)user);
});

app.MapGet("/user/login", (string username, string password) =>
{
    var dto = new CreateUserDTO
    {
        Username = username,
        Password = password
    };

    app.Logger.LogDebug("Login attempt for user {username}", username);

    if (!Validator.TryValidateObject(dto, new(dto), [], true))
    {
        app.Logger.LogError("Login request denied for user {username} - credential validation failed.", username);

        return Results.BadRequest("Incorrect credentials.");
    }

    if (users.SingleOrDefault(x => x.Username == username) is not { } user)
    {
        app.Logger.LogError("Login request denied for user {username} - no user found.", username);

        return Results.NotFound("No user with such username.");
    }

    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));

    if (user.PasswordHash != Encoding.UTF8.GetString(hash))
    {
        app.Logger.LogError("Login request denied for user {username} - password hash mismatch.", username);

        return Results.BadRequest("Incorrect password.");
    }

    return Results.Ok(new
    {
        User = (UserDTO)user,
        AccessToken = Guid.NewGuid()
    });
});

app.MapGet("/users", () =>
{
    app.Logger.LogDebug("The user list has been accessed. Returned {count} users.", users.Count);

    return users.Select(x => (UserDTO)x);
});

app.MapGet("/users/{username}", (string username) =>
{
    app.Logger.LogDebug("Searching for users with username {username}", username);

    var user = users.SingleOrDefault(x => x.Username == username);

    if (user is null)
    {
        app.Logger.LogError("User with username {username} was not found.", username);

        return Results.NotFound("No user with such username.");
    }

    app.Logger.LogDebug("User {username} was found. ID = {id}", username, user.Id);

    return Results.Ok((UserDTO)user);
});

app.Run();