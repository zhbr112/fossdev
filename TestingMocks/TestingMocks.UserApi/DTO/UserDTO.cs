using TestingMocks.Models;

namespace TestingMocks.UserApi.DTO;

public record UserDTO(string Username)
{
    public static explicit operator UserDTO(User user)
    {
        return new(user.Username);
    }
}