using Shared.EventBus.Entities;

namespace Shared.EventBus.Interfaces
{
	public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
	where TIntegrationEvent : IntegrationEvent
	{
		Task Handle(TIntegrationEvent @event);
	}

	public interface IIntegrationEventHandler
	{
	}
}
