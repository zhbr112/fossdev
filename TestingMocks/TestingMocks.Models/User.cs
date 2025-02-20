using System.ComponentModel.DataAnnotations;

namespace TestingMocks.Models;

public class User
{
    [Key, StringLength(32, MinimumLength = 3)]
    public required string Username { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    public UserDetails? Details { get; set; }
}
