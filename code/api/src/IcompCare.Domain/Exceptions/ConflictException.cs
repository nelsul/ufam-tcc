using IcompCare.Domain.Constants;

namespace IcompCare.Domain.Exceptions;

public class ConflictException : DomainException
{
    public ConflictException(string message)
        : base(message, DomainErrorCodes.Conflict) { }

    public ConflictException(string message, string errorCode)
        : base(message, errorCode) { }
}
