using Common.Exceptions;

namespace Users.Domain.Exceptions;

public class InvalidUserNameException : InvalidValueException
{
    public InvalidUserNameException(string message) : base(message) { }
}