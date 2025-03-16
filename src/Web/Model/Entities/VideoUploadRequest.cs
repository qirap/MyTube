using Microsoft.AspNetCore.Components.Forms;

namespace Web.Model.Entities
{
	public class VideoUploadRequest
	{
		public string Title { get; set; } = null!;
		public string Description { get; set; } = null!;
		public IBrowserFile? File { get; set; }
	}
}
