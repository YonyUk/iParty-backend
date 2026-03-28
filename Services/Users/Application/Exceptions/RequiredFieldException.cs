using Common.Exceptions;

namespace Users.Application.Exceptions;

public class RequiredFieldException : RequiredException
{
    public RequiredFieldException(string fieldName)
    :base($"Required field {fieldName}")
    {
        
    }
}