using IcompCare.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IcompCare.Infrastructure.Email;

public class EmailBackgroundService : BackgroundService
{
    private readonly EmailQueueService _emailQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailBackgroundService> _logger;
    private readonly EmailSettings _settings;

    public EmailBackgroundService(
        EmailQueueService emailQueue,
        IServiceScopeFactory scopeFactory,
        ILogger<EmailBackgroundService> logger,
        IOptions<EmailSettings> settings
    )
    {
        _emailQueue = emailQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Email Background Service started");

        await foreach (var message in _emailQueue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                await emailSender.SendEmailAsync(
                    message.To,
                    message.Subject,
                    message.HtmlBody,
                    message.FromName,
                    stoppingToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process email to {To}. Retry count: {RetryCount}",
                    message.To,
                    message.RetryCount
                );

                if (message.RetryCount < _settings.MaxRetryAttempts)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(_settings.RetryDelaySeconds),
                        stoppingToken
                    );

                    var retryMessage = message with { RetryCount = message.RetryCount + 1 };
                    _emailQueue.QueueEmail(retryMessage);
                }
                else
                {
                    _logger.LogError(
                        "Email to {To} failed after {MaxRetries} attempts. Giving up.",
                        message.To,
                        _settings.MaxRetryAttempts
                    );
                }
            }
        }

        _logger.LogInformation("Email Background Service stopped");
    }
}
