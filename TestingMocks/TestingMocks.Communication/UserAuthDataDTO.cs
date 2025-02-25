using System.ComponentModel.DataAnnotations;

namespace TestingMocks.Communication;

/// <summary>
/// Аутентификационные данные пользователя
/// </summary>
public record UserAuthDataDTO
{
    /// <summary>
    /// Создать аутентификационные данные пользователя
    /// </summary>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    public UserAuthDataDTO(string username, string password)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    /// <remarks>
    /// Длина от 3 до 32 символов
    /// </remarks>
    [StringLength(32, MinimumLength = 3)]
    public string Username { get; init; }

    /// <summary>
    /// Пароль
    /// </summary>
    /// <remarks>
    /// Длина от 8 до 32 символов
    /// </remarks>
    [StringLength(32, MinimumLength = 8)]
    public string Password { get; init; }
}
