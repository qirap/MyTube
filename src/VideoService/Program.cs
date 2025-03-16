using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Shared.Common;
using Shared.EventBus.Implementations;
using Shared.EventBus.Interfaces;
using Shared.S3Storage;
using Shared.S3Storage.Interfaces;
using Shared.Security.Implementations;
using VideoService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
	options.Limits.MaxRequestBodySize = 500 * 1024 * 1024; // 500MB
});

builder.Services.AddDbContext<VideoContext>(options =>
{
	options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

builder.Services.AddSingleton<IEventBus>(provider =>
{
	return new KafkaEventBus(builder.Configuration["Kafka:Url"]);
});

builder.Services.Configure<S3ProviderConfiguration>(builder.Configuration.GetSection("Amazon:S3"));

builder.Services.AddAmazonS3Provider();

builder.Services.AddAuthentication("MyTubeScheme")
	.AddScheme<AuthenticationSchemeOptions, MyTubeAuthenticationHandler>("MyTubeScheme", null);
builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var s3Provider = scope.ServiceProvider.GetRequiredService<IS3Provider>();
	await s3Provider.EnsureBucketExistsAsync();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
