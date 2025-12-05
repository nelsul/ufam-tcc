using System.Net;
using System.Net.Mail;
using IcompCare.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IcompCare.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailSettings> settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        await SendEmailAsync(to, subject, htmlBody, null, cancellationToken);
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string htmlBody,
        string? fromName = null,
        CancellationToken cancellationToken = default
    )
    {
        await SendEmailInternalAsync(new[] { to }, subject, htmlBody, fromName, cancellationToken);
    }

    public async Task SendEmailAsync(
        IEnumerable<string> to,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default
    )
    {
        await SendEmailInternalAsync(to, subject, htmlBody, null, cancellationToken);
    }

    private async Task SendEmailInternalAsync(
        IEnumerable<string> recipients,
        string subject,
        string htmlBody,
        string? fromName,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrEmpty(_settings.SmtpHost))
        {
            _logger.LogWarning(
                "Email not configured. Skipping email to {Recipients} with subject: {Subject}",
                string.Join(", ", recipients),
                subject
            );
            return;
        }

        try
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword),
                EnableSsl = _settings.EnableSsl,
            };

            var displayName = fromName ?? _settings.FromName;
            var fromAddress = new MailAddress(_settings.FromEmail, displayName);

            using var message = new MailMessage
            {
                From = fromAddress,
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };

            foreach (var recipient in recipients)
            {
                message.To.Add(recipient);
            }

            await client.SendMailAsync(message, cancellationToken);

            _logger.LogInformation(
                "Email sent successfully to {Recipients} with subject: {Subject}",
                string.Join(", ", recipients),
                subject
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send email to {Recipients} with subject: {Subject}",
                string.Join(", ", recipients),
                subject
            );
            throw;
        }
    }
}
