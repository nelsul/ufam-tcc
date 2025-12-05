namespace IcompCare.Domain.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    );
    Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        string? fromName = null,
        CancellationToken cancellationToken = default
    );
    Task SendEmailAsync(
        IEnumerable<string> to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    );
}
