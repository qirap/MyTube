using Shared.EventBus.Entities;

namespace Shared.EventBus.Interfaces
{
	public interface IEventBus
	{
		Task Publish(IntegrationEvent @event);

		Task Subscribe<T, TH>()
			where T : IntegrationEvent
			where TH : IIntegrationEventHandler<T>;
	}
}
