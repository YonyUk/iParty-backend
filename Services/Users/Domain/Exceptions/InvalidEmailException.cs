using Common.Exceptions;

namespace Users.Domain.Exceptions;

public class InvalidEmailException : InvalidValueException
{
    public InvalidEmailException(string message) : base(message) { }
}