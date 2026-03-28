using Common.Exceptions;

namespace Users.Domain.Exceptions;

public class InvalidHashedPasswordException : InvalidValueException
{
    public InvalidHashedPasswordException(string message) : base(message) { }
}