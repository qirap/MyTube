namespace Shared.S3Storage.Interfaces
{
	public interface IS3Provider
	{
		Task<IEnumerable<string>> GetBuckets(CancellationToken cancellationToken = default);
		Task UploadFileAsync(string bucketName, string fileName, Stream fileStream, string contentType);
		Task<List<string>> ListFilesInFolderAsync(string bucketName, string folderName);
		Task<string> GeneratePresignedUrlAsync(string bucketName, string objectKey, int expirationMinutes = 60);
		Task DeleteFileAsync(string bucketName, string fileName);
		Task EnsureBucketExistsAsync();
	}
}
