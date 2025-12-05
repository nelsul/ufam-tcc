namespace IcompCare.Application.DTOs.Email;

public record EmailMessage
{
    public required string To { get; init; }
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
    public string? FromName { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public int RetryCount { get; init; } = 0;
}

public record EmailMessageBatch
{
    public required IEnumerable<string> To { get; init; }
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
    public string? FromName { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
