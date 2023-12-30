using Mukesh.Shared.Events;

namespace Mukesh.Domain.Common.Contracts;
public abstract class DomainEvent : IEvent
{
    public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}