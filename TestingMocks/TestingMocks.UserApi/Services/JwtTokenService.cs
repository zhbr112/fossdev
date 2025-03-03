using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestingMocks.Models;
using TestingMocks.UserApi.Configuration;

namespace TestingMocks.UserApi.Services;

/// <summary>
/// Сервис для создания jwt
/// </summary>
/// <param name="jwtHandler">Используемый сервис для генерации токенов</param>
/// <param name="options">Конфигурация jwt</param>
public class JwtTokenService(JwtSecurityTokenHandler jwtHandler, IOptions<AuthConfiguration> options)
{
    private readonly JwtConfiguration jwtOptions = options.Value.Jwt;

    /// <summary>
    /// Сгенерировать токен
    /// </summary>
    /// <param name="user">пользователь</param>
    /// <returns>Токен</returns>
    public string GenerateAccessToken(User user)
    {
        var jwt = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: [
                new Claim(ClaimTypes.Name, user.Username)
            ],
            expires: DateTime.UtcNow.Add(jwtOptions.Expiration),
            signingCredentials: jwtOptions.SigningCredentials
        );

        return jwtHandler.WriteToken(jwt);
    }
}