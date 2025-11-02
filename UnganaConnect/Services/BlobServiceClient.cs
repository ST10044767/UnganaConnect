using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace UnganaConnect.Frontend.Services
{
    public class AzureBlobService
    {
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "resources";

    public AzureBlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadAsync(IFormFile file, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + "-" + file.FileName);
        await using var stream = file.OpenReadStream();

        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

        return blobClient.Uri.ToString(); // public URL
    }
     public async Task<string> FileUploadAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            return blobClient.Uri.ToString();
        }

        public async Task DeleteAsync(string blobUrl)
        {
            try
            {
                var uri = new Uri(blobUrl);
                var containerName = uri.Segments[1].TrimEnd('/');
                var blobName = string.Join("/", uri.Segments.Skip(2).Select(s => s.TrimEnd('/')));
                
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
            }
            catch
            {
                // If parsing fails, try default container
                try
                {
                    var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                    var blobName = Path.GetFileName(new Uri(blobUrl).LocalPath);
                    var blobClient = containerClient.GetBlobClient(blobName);
                    await blobClient.DeleteIfExistsAsync();
                }
                catch
                {
                    // Log error but don't throw - deletion is not critical
                }
            }
        }
    }
}

