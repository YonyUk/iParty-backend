using System.ComponentModel.DataAnnotations;
using Users.Domain.Rules;

namespace Users.API.DTOs;

public record ChangePasswordDTO
{
    [Required]
    public string Password { get; init; } = string.Empty;
}