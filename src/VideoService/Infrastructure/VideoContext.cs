using Microsoft.EntityFrameworkCore;
using Shared.Model.Entities;

namespace VideoService.Infrastructure
{
	public class VideoContext : DbContext
	{
		public VideoContext(DbContextOptions<VideoContext> options) : base(options)
		{
			Database.EnsureCreated();
		}
		public DbSet<VideoItem> Videos { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}
