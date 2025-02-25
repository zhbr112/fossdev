using TestingMocks.Models;

namespace TestingMocks.Communication;

/// <summary>
/// Данные о пользователе
/// </summary>
/// <param name="Username">Имя пользователя</param>
/// <param name="Details">Информация о пользователе</param>
public record UserDTO(string Username, UserDetailsDTO? Details)
{
    /// <summary>
    /// Создать данные о пользователе из модели БД пользователя
    /// </summary>
    /// <param name="user">Данные о пользователе</param>
    public static explicit operator UserDTO(User user)
    {
        return new(user.Username, (UserDetailsDTO?)user.Details);
    }
}