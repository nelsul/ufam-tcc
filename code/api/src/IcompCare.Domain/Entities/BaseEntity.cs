using System;

namespace IcompCare.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
