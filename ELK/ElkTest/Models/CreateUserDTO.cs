namespace ElkTest.Models;

using System.ComponentModel.DataAnnotations;

public record CreateUserDTO
{
    [Required, StringLength(16, MinimumLength = 3, ErrorMessage = "The username must consist of 3 to 16 characters")]
    [RegularExpression(@"^[a-zA-Z0-9]{3,16}$", ErrorMessage = "The username must only consist of alphanumeric characters")]
    public required string Username { get; set; }
    [Required, StringLength(64, MinimumLength = 8, ErrorMessage = "The password must consist of 8 to 64 characters")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,64}$", ErrorMessage = "The password must consist of at least one letter, one number and one special character (@$!%*#?&)")]
    public required string Password { get; set; }
}