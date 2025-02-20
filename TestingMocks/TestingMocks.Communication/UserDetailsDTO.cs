using System.ComponentModel.DataAnnotations;
using TestingMocks.Models;

namespace TestingMocks.Communication;

public record UserDetailsDTO
{
    [StringLength(64, MinimumLength = 2)]
    public string? Name { get; init; }

    [Range(0, 128)]
    public int? Age { get; init; }

    [StringLength(128, MinimumLength = 2)]
    public string? City { get; init; }

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