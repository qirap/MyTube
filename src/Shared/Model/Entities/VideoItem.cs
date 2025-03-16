namespace Shared.Model.Entities
{
	public class VideoItem
	{
		public string Id { get; set; } = Guid.NewGuid().ToString();
		public string UserId { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Path { get; set; } = string.Empty;
	}
}
