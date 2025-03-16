using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared.S3Storage.Interfaces;
using System.Threading.Tasks;

namespace Shared.S3Storage.Implementations
{
	public class AmazonS3Provider : IS3Provider
	{
		private readonly AmazonS3Client s3Client;
		public AmazonS3Provider(IOptions<S3ProviderConfiguration> options)
		{
			var s3ProviderConfig = options.Value;
			var credentials = new BasicAWSCredentials(s3ProviderConfig.AccessKey, s3ProviderConfig.SecretKey);
			var s3Config = new AmazonS3Config
			{
				ServiceURL = s3ProviderConfig.Url,
				ForcePathStyle = true
			};
			s3Client = new AmazonS3Client(credentials, s3Config);
		}

		public async Task<IEnumerable<string>> GetBuckets(CancellationToken cancellationToken = default)
		{
			var buckets = await s3Client.ListBucketsAsync(cancellationToken);
			return buckets.Buckets.Select(b => b.BucketName);
		}

		public async Task UploadFileAsync(string bucketName, string fileName, Stream fileStream, string contentType)
		{
			var putRequest = new PutObjectRequest
			{
				BucketName = bucketName,
				Key = fileName,
				InputStream = fileStream,
				ContentType = contentType,
				CannedACL = S3CannedACL.PublicRead
			};

			await s3Client.PutObjectAsync(putRequest);
		}

		public async Task DeleteFileAsync(string bucketName, string fileName)
		{
			try
			{
				var deleteRequest = new DeleteObjectRequest
				{
					BucketName = bucketName,
					Key = fileName
				};

				await s3Client.DeleteObjectAsync(deleteRequest);
				Console.WriteLine($"Файл {fileName} успешно удален из бакета {bucketName}");
			}
			catch (AmazonS3Exception ex)
			{
				Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
				throw;
			}
		}

		public async Task<List<string>> ListFilesInFolderAsync(string bucketName, string folderName)
		{
			var request = new ListObjectsV2Request
			{
				BucketName = bucketName,
				Prefix = folderName + "/",
			};

			var response = await s3Client.ListObjectsV2Async(request);

			var files = new List<string>();
			foreach (var obj in response.S3Objects)
			{
				files.Add(obj.Key);
			}

			return files;
		}

		public async Task<string> GeneratePresignedUrlAsync(string bucketName, string objectKey, int expirationMinutes = 60)
		{
			var request = new GetPreSignedUrlRequest
			{
				BucketName = bucketName,
				Key = objectKey,
				Expires = DateTime.UtcNow.AddMinutes(expirationMinutes)
			};

			return await s3Client.GetPreSignedURLAsync(request);
		}

		public async Task EnsureBucketExistsAsync()
		{
			var bucketName = "mytube";
			try
			{
				var response = await s3Client.ListBucketsAsync();
				if (!response.Buckets.Exists(b => b.BucketName == bucketName))
				{
					await s3Client.PutBucketAsync(new PutBucketRequest { BucketName = bucketName });
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Ошибка при проверке/создании бакета {bucketName}: {ex.Message}", ex);
			}
		}
	}
}
