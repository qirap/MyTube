namespace VideoService.Model.Entities
{
	public class VideoUploadDto
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public IFormFile File { get; set; }
	}
}
