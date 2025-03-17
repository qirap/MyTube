using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.EventBus.IntegrationEvents;
using Shared.EventBus.Interfaces;
using Shared.Model.Entities;
using Shared.S3Storage.Interfaces;
using System.Security.Claims;
using VideoService.Infrastructure;
using VideoService.Model.Entities;

namespace VideoService.Controllers
{
	[Route("api/video")]
	[ApiController]
	[Authorize]
	public class VideoController(
		IS3Provider s3Provider,
		VideoContext videoContext,
		IEventBus eventBus
		) : ControllerBase
	{
		[ProducesResponseType(StatusCodes.Status200OK)]
		[HttpGet("getall")]
		public async Task<IActionResult> GetAllVideosAsync()
		{
			var videos = await videoContext.Videos.ToListAsync();
			
			return Ok(videos);
		}

		[ProducesResponseType(StatusCodes.Status200OK)]
		[HttpGet("getuservideos")]
		public async Task<IActionResult> GetUserVideosAsync()
		{
			var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).ToString();
			var videos = await videoContext.Videos.Where(v => v.UserId == userId).ToListAsync();

			return Ok(videos);
		}

		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost("upload")]
		public async Task<IActionResult> UploadVideo([FromForm] VideoUploadDto request)
		{
			if (request.File == null || request.File.Length == 0)
				return BadRequest("File is required.");

			using var stream = request.File.OpenReadStream();
			var s3ObjectKey = "videos/" + User.Identity.Name + "/" + request.File.FileName;
			await s3Provider.UploadFileAsync(
				"mytube",
				s3ObjectKey,
				stream,
				"video/mp4");

			var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).ToString();
			var videoItem = new VideoItem
			{
				UserId = userId,
				Title = request.Title,
				Description = request.Description,
				Path = s3ObjectKey
			};
			await videoContext.AddAsync(videoItem);
			await videoContext.SaveChangesAsync();

			await eventBus.Publish(new VideoUploadedIntegrationEvent
			{
				VideoId = videoItem.Id,
				UserId = videoItem.UserId,
				Path = videoItem.Path
			});

			return Ok();
		}

		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> DeleteVideo(string id)
		{
			var video = await videoContext.Videos.FirstOrDefaultAsync(v => v.Id == id);
			if (video == null)
			{
				return NotFound();
			}

			await s3Provider.DeleteFileAsync("mytube", video.Path);

			videoContext.Remove(video);
			await videoContext.SaveChangesAsync();

			return Ok();
		}

		[ProducesResponseType(StatusCodes.Status200OK)]
		[HttpPost("getlink")]
		public async Task<IActionResult> GetLink([FromBody] string videoId)
		{
			var videoInfo = videoContext.Videos.FirstOrDefault(v => v.Id == videoId);
			if (videoInfo == null)
			{
				return NotFound();
			}

			var link = await s3Provider.GeneratePresignedUrlAsync("mytube", videoInfo.Path);
			link = link.Replace("https://minio", "http://localhost");

			return Ok(link);
		}
	}
}
