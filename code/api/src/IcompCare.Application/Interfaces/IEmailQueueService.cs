using IcompCare.Application.DTOs.Email;

namespace IcompCare.Application.Interfaces;

public interface IEmailQueueService
{
    void QueueEmail(string to, string subject, string htmlBody, string? fromName = null);
    void QueueEmail(EmailMessage message);
    void QueueEmails(
        IEnumerable<string> to,
        string subject,
        string htmlBody,
        string? fromName = null
    );
}
