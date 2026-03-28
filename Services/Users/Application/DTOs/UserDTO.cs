using Users.Domain;

namespace Users.Application.DTOs;

public record UserDTO(Guid Id,string UserName,string Email,UserRole Role);