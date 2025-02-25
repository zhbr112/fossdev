using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TestingMocks.UserApi.Configuration;

public record AuthConfiguration
{
    [Required, StringLength(512, MinimumLength = 32)]
    public string PasswordSalt { get; init; } = default!;

    public byte[] PasswordSaltBytes => Encoding.ASCII.GetBytes(PasswordSalt);

    [Required]
    public required JwtConfiguration Jwt { get; init; }
}