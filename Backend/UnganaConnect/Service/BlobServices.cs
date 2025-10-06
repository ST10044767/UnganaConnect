using Microsoft.AspNetCore.Hosting;
using Azure.Storage.Blobs;
using Azure;

namespace UnganaConnect.Service
{
    public class BlobServices
    {
        private readonly string _webRootPath;
        private readonly BlobContainerClient? _containerClient;

        public BlobServices(IWebHostEnvironment env)
        {
            _webRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var connectionString = Environment.GetEnvironmentVariable("AzureBlob");
            var containerName = Environment.GetEnvironmentVariable("AzureContainer") ?? "uploads";
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                _containerClient = new BlobContainerClient(connectionString, containerName);
                try { _containerClient.CreateIfNotExists(); } catch (RequestFailedException) { /* ignore */ }
            }
        }

        public async Task<string> UploadAsync(Stream stream, string fileName, string folder = "uploads")
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("fileName required", nameof(fileName));

            var safeFileName = $"{Guid.NewGuid()}_{fileName}";

            if (_containerClient != null)
            {
                var blobClient = _containerClient.GetBlobClient($"{folder}/{safeFileName}");
                await blobClient.UploadAsync(stream, overwrite: true);
                return blobClient.Uri.ToString();
            }
            else
            {
                var uploadsFolder = Path.Combine(_webRootPath, folder);
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, safeFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await stream.CopyToAsync(fileStream);
                }
                return $"/{folder}/{safeFileName}";
            }
        }
    }
}
