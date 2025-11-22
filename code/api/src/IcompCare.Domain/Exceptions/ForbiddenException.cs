using IcompCare.Domain.Constants;

namespace IcompCare.Domain.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException(string message)
        : base(message, DomainErrorCodes.Common.Forbidden) { }
}
