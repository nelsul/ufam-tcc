using IcompCare.Domain.Constants;

namespace IcompCare.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", DomainErrorCodes.NotFound) { }

    public NotFoundException(string message, string errorCode)
        : base(message, errorCode) { }
}
