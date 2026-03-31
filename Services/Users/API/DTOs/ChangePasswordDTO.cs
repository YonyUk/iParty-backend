using System.ComponentModel.DataAnnotations;
using Users.Domain.Rules;

namespace Users.API.DTOs;

public record ChangePasswordDTO
{
    [Required]
    public string Password { get; init; } = string.Empty;
    [Required]
    [Compare("Password",ErrorMessage = "Passwords doesn't matches")]
    public string Confirm { get; init; } = string.Empty;
}