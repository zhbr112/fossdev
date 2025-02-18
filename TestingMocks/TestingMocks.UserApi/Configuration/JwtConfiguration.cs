using System.ComponentModel.DataAnnotations;

namespace TestingMocks.UserApi.Configuration;

public record JwtConfiguration
{
    [Required, StringLength(512, MinimumLength = 32)]
    public required string SigningKey { get; init; }

    [Required]
    public TimeSpan Expiration { get; init; } = TimeSpan.FromDays(1);
}