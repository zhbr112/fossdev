namespace ElkTest.Models;

public record UserDTO(Guid Id, string Username, DateTime CreatedAt)
{
    public static explicit operator UserDTO(User user)
    {
        return new(user.Id, user.Username, user.CreatedAt);
    }
}