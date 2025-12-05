using System.Threading.Channels;
using IcompCare.Application.DTOs.Email;
using IcompCare.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace IcompCare.Infrastructure.Email;

public class EmailQueueService : IEmailQueueService
{
    private readonly Channel<EmailMessage> _emailChannel;
    private readonly ILogger<EmailQueueService> _logger;

    public EmailQueueService(ILogger<EmailQueueService> logger)
    {
        _logger = logger;
        _emailChannel = Channel.CreateUnbounded<EmailMessage>(
            new UnboundedChannelOptions { SingleReader = true, SingleWriter = false }
        );
    }

    public ChannelReader<EmailMessage> Reader => _emailChannel.Reader;

    public void QueueEmail(string to, string subject, string htmlBody, string? fromName = null)
    {
        var message = new EmailMessage
        {
            To = to,
            Subject = subject,
            HtmlBody = htmlBody,
            FromName = fromName,
        };
        QueueEmail(message);
    }

    public void QueueEmail(EmailMessage message)
    {
        if (!_emailChannel.Writer.TryWrite(message))
        {
            _logger.LogWarning(
                "Failed to queue email to {To} with subject: {Subject}",
                message.To,
                message.Subject
            );
        }
        else
        {
            _logger.LogDebug(
                "Email queued to {To} with subject: {Subject}",
                message.To,
                message.Subject
            );
        }
    }

    public void QueueEmails(
        IEnumerable<string> to,
        string subject,
        string htmlBody,
        string? fromName = null
    )
    {
        foreach (var recipient in to)
        {
            QueueEmail(recipient, subject, htmlBody, fromName);
        }
    }
}
