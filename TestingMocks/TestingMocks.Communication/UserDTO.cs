using TestingMocks.Models;

namespace TestingMocks.Communication;

public record UserDTO(string Username, UserDetailsDTO? Details)
{
    public static explicit operator UserDTO(User user)
    {
        return new(user.Username, (UserDetailsDTO?)user.Details);
    }
}