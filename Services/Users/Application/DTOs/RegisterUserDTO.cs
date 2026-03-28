using Users.Domain;

namespace Users.Application.DTOs;

public record RegisterUserDTO(string UserName,string Email,string Password,UserRole Role);