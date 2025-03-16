using Shared.EventBus.Entities;

namespace Shared.EventBus.IntegrationEvents
{
	public class VideoUploadedIntegrationEvent : IntegrationEvent
	{
		public string VideoId { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
		public string Path { get; set; } = string.Empty;
	}
}
