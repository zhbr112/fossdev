using TestingMocks.Models;

namespace TestingMocks.Communication;

public record UserDetailsDTO(string? Name, byte Age, string? City)
{
    public static explicit operator UserDetailsDTO?(UserDetails? userDetails)
    {
        if (userDetails is null) return null;

        return new(
            Name: userDetails.Name,
            Age: userDetails.Age,
            City: userDetails.City
        );
    }
}