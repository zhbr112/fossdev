using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TestingMocks.Models;

[Owned]
public class UserDetails
{
    [StringLength(64, MinimumLength = 2)]
    public string? Name { get; set; }

    [Range(0, 128)]
    public byte Age { get; set; }

    [StringLength(128, MinimumLength = 2)]
    public string? City { get; set; }
}