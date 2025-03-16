using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.S3Storage;
using Shared.S3Storage.Interfaces;
using Shared.S3Storage.Implementations;

namespace Shared.Common
{
	public static class ServiceProviderExtensions
	{
		public static IServiceCollection AddAmazonS3Provider(this IServiceCollection services)
		{
			services.AddScoped<IS3Provider>(provider =>
			{
				var options = provider.GetRequiredService<IOptions<S3ProviderConfiguration>>();
				return new AmazonS3Provider(options);
			});

			return services;
		}
	}
}
