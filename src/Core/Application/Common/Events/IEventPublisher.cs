using ICISAdminPortal.Shared.Events;

namespace ICISAdminPortal.Application.Common.Events;
public interface IEventPublisher : ITransientService
{
    Task PublishAsync(IEvent @event);
}