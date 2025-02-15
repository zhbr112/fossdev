using System.ComponentModel.DataAnnotations;

namespace TestingMocks.Models;

public class User
{
    [Key, StringLength(32, MinimumLength = 3)]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
