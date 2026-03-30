using Common.Exceptions;

namespace Users.Domain.Exceptions;

public class InvalidUserRoleException : InvalidValueException
{
    public InvalidUserRoleException(string role)
    : base($"{role} is not a valid value for field Role")
    {
        
    }
}