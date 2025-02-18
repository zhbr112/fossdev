using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TestingMocks.UserApi.Configuration;

public record JwtConfiguration
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }

    [Required, StringLength(512, MinimumLength = 32)]
    public required string SigningKey { get; init; }

    [Required]
    public TimeSpan Expiration { get; init; } = TimeSpan.FromDays(1);

    public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.ASCII.GetBytes(SigningKey));

    public SigningCredentials SigningCredentials => new(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
}