using Shared.EventBus.IntegrationEvents;
using Shared.EventBus.Interfaces;

namespace NotificationService.IntegrationEvents.EventHandlers
{
	public class VideoUploadedIntegrationEventHandler : IIntegrationEventHandler<VideoUploadedIntegrationEvent>
	{
		public async Task Handle(VideoUploadedIntegrationEvent @event)
		{
			Console.WriteLine("Message handled:");
			Console.WriteLine($"User id: {@event.UserId}");
			Console.WriteLine($"Video id: {@event.VideoId}");
			Console.WriteLine($"Video path: {@event.Path}");
		}
	}
}
