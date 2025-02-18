using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TestingMocks.UserApi.Configuration;
using TestingMocks.UserApi.Data;
using TestingMocks.UserApi.DTO;
using TestingMocks.UserApi.Exceptions;
using TestingMocks.UserApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Настройки
builder.Services.AddOptions<AuthConfiguration>()
    .Bind(builder.Configuration.GetSection("Auth"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandler>();

// БД
builder.Services.AddDbContext<UserDbContext>(opt =>
{
    if (builder.Environment.IsDevelopment()) opt.UseInMemoryDatabase("DevDB");
    else throw new NotImplementedException("No production DB assigned yet.");
}, builder.Environment.IsDevelopment() ? ServiceLifetime.Singleton : ServiceLifetime.Scoped);

// Сервисы
builder.Services.AddScoped<UserService>();
builder.Services.AddTransient<PasswordHasherService>();

// Runtime-конфигурация
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseExceptionHandler();
}

// Эндпоинты
var authGroup = app.MapGroup("/auth");

authGroup.MapPost("/register", async (UserAuthDataDTO userDTO, UserService users) =>
{
    var user = await users.RegisterAsync(userDTO.Username, userDTO.Password);

    return Results.Ok((UserDTO)user);
});

app.Run();

