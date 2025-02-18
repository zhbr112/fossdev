using System.ComponentModel.DataAnnotations;

namespace TestingMocks.UserApi.Configuration;

public record AuthConfiguration
{
    [Required, StringLength(512, MinimumLength = 32)]
    public required string PasswordSalt { get; init; }

    [Required]
    public required JwtConfiguration Jwt { get; init; }
}