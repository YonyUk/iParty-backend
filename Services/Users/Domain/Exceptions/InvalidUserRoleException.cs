using Common.Exceptions;

namespace Users.Domain.Exceptions;

public class InvalidUserRoleException : InvalidValueException
{
    public InvalidUserRoleException(string role,string[] validNames)
    : base($"Value {role} is not a valid value for field Role. Valid values: {string.Join(", ",validNames)}")
    {
        
    }

    public InvalidUserRoleException(int role, int[] validValues)
    : base($"Value {role} is not a numeric value valid for field Role. Valid values: {string.Join(", ",validValues)}")
    {
        
    }
}