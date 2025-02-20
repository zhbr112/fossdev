using System.ComponentModel.DataAnnotations;

namespace TestingMocks.Communication;

public record UserAuthDataDTO
{
    public UserAuthDataDTO(string username, string password)
    {
        Username = username;
        Password = password;
    }

    [StringLength(32, MinimumLength = 3)]
    public string Username { get; init; }

    [StringLength(32, MinimumLength = 8)]
    public string Password { get; init; }
}
