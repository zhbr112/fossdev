using System.ComponentModel.DataAnnotations;
using TestingMocks.Models;

namespace TestingMocks.Communication;

/// <summary>
/// Детальная информация о пользователе
/// </summary>
public record UserDetailsDTO
{
    /// <summary>
    /// Фактическое имя пользователя
    /// </summary>
    /// <remarks>
    /// Длина от 2 до 64 символов
    /// </remarks>
    [StringLength(64, MinimumLength = 2)]
    public string? Name { get; init; }

    /// <summary>
    /// Возраст пользователя
    /// </summary>
    /// <remarks>
    /// Значения от 0 до 128
    /// </remarks>
    [Range(0, 128)]
    public int? Age { get; init; }

    /// <summary>
    /// Город проживания пользователя
    /// </summary>
    /// <remarks>
    /// Длина от 2 до 128 символов
    /// </remarks>
    [StringLength(128, MinimumLength = 2)]
    public string? City { get; init; }

    /// <summary>
    /// Создать DTO из модели БД
    /// </summary>
    /// <param name="userDetails">Данные о пользователе</param>
    public static explicit operator UserDetailsDTO?(UserDetails? userDetails)
    {
        if (userDetails is null) return null;

        return new()
        {
            Name = userDetails.Name,
            Age = userDetails.Age,
            City = userDetails.City
        };
    }
}