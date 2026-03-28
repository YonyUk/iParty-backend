using Common.Exceptions;

namespace Users.Domain.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string fieldName,string fieldValue)
    :base($"A user with {fieldName} '{fieldValue}' was not found")
    {
        
    }
}