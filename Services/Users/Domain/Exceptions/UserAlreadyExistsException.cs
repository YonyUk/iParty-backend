using Common.Exceptions;

namespace Users.Domain.Exceptions;

public class UserAlreadyExistsException : AlreadyExistsException
{
    public UserAlreadyExistsException(string fieldName,string fieldValue)
    :base($"A user with {fieldName} '{fieldValue}' already exists")
    {
        
    }
}