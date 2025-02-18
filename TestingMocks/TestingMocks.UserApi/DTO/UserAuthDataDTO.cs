using System.ComponentModel.DataAnnotations;

namespace TestingMocks.UserApi.DTO;

public record UserAuthDataDTO([StringLength(32, MinimumLength = 3)] string Username, [StringLength(32, MinimumLength = 8)] string Password)
{

}
