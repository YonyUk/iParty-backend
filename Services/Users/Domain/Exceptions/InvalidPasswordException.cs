using Common.Exceptions;

namespace Users.Domain.Exceptions;

public class InvalidPasswordException : InvalidValueException
{
    public InvalidPasswordException(string message) : base(message) { }
}