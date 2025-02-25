using System.ComponentModel.DataAnnotations;

namespace TestingMocks.Models;

/// <summary>
/// Сущность пользователя
/// </summary>
public class User
{
    /// <summary>
    /// Логин пользователя
    /// </summary>
    /// <remarks>
    /// Длина от 3 до 32 символов
    /// </remarks>
    [Key, StringLength(32, MinimumLength = 3)]
    public required string Username { get; set; }

    /// <summary>
    /// Захэшированный пароль
    /// </summary>
    [Required]
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public UserDetails? Details { get; set; }
}
