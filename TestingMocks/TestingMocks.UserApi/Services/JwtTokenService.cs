using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestingMocks.Models;
using TestingMocks.UserApi.Configuration;

namespace TestingMocks.UserApi.Services;

public class JwtTokenService(JwtSecurityTokenHandler jwtHandler, IOptions<AuthConfiguration> options)
{
    private readonly JwtConfiguration jwtOptions = options.Value.Jwt;

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