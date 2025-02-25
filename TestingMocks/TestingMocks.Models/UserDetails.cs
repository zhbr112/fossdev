using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TestingMocks.Models;

/// <summary>
/// Сущность информации о пользователе
/// </summary>
/// <remarks>
/// Привязана к сущности User
/// </remarks>
/// <seealso cref="User"/>
[Owned]
public class UserDetails
{
    /// <summary>
    /// Фактическое имя пользователя
    /// </summary>
    /// <remarks>
    /// Длина от 2 до 64 символов
    /// </remarks>
    [StringLength(64, MinimumLength = 2)]
    public string? Name { get; set; }

    /// <summary>
    /// Возраст пользователя
    /// </summary>
    /// <remarks>
    /// Принимает значения от 0 до 128
    /// </remarks>
    [Range(0, 128)]
    public byte Age { get; set; }

    /// <summary>
    /// Город проживания пользователя
    /// </summary>
    /// <remarks>
    /// Длина от 2 до 128 символов
    /// </remarks>
    [StringLength(128, MinimumLength = 2)]
    public string? City { get; set; }
}