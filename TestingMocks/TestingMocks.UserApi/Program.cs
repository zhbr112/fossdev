using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var jwtConfig = builder.Configuration.GetSection("Auth:Jwt").Get<JwtConfiguration>()!;

        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = jwtConfig.Issuer is not null,
            ValidIssuer = jwtConfig.Issuer,
            ValidateAudience = jwtConfig.Audience is not null,
            ValidAudience = jwtConfig.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = jwtConfig.SymmetricSecurityKey,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization();

// БД
builder.Services.AddDbContext<UserDbContext>(opt =>
{
    if (builder.Environment.IsDevelopment()) opt.UseInMemoryDatabase("DevDB");
    else throw new NotImplementedException("No production DB assigned yet.");
}, builder.Environment.IsDevelopment() ? ServiceLifetime.Singleton : ServiceLifetime.Scoped);

// Сервисы
builder.Services.AddTransient<JwtSecurityTokenHandler>();
builder.Services.AddScoped<UserService>();
builder.Services.AddTransient<PasswordHasherService>();
builder.Services.AddTransient<JwtTokenService>();

// Runtime-конфигурация
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseExceptionHandler();
}

// Эндпоинты
var authGroup = app.MapGroup("/auth");

authGroup.MapPost("/register", async (UserAuthDataDTO authData, UserService users) =>
{
    var user = await users.RegisterAsync(authData.Username, authData.Password);

    return (UserDTO)user;
});

authGroup.MapPost("/login", async (UserAuthDataDTO authData, UserService users, JwtTokenService jwt) =>
{
    var user = await users.LoginAsync(authData.Username, authData.Password);

    var token = jwt.GenerateAccessToken(user);

    return new
    {
        User = (UserDTO)user,
        Token = token
    };
});

var usersGroup = app.MapGroup("/users");

usersGroup.MapGet("/me", async (UserService users, ClaimsPrincipal claims) =>
{
    var username = (claims.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name)?.Value)
        ?? throw new UnauthorizedAccessException();

    var user = await users.GetUserAsync(username);

    return (UserDTO)user;
}).RequireAuthorization();

app.Run();

