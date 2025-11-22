using IcompCare.Domain.Constants;

namespace IcompCare.Domain.Exceptions;

public class ValidationException : DomainException
{
    public ValidationException(string message)
        : base(message, DomainErrorCodes.Validation) { }

    public ValidationException(string message, string errorCode)
        : base(message, errorCode) { }
}
