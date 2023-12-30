using Mukesh.Shared.Events;

namespace Mukesh.Application.Common.Events;
public interface IEventPublisher : ITransientService
{
    Task PublishAsync(IEvent @event);
}