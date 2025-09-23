using Azure.Storage.Blobs;
using DotNetEnv;

namespace UnganaConnect.Service
{
    public class BlobService
    {
        private readonly BlobContainerClient _container;

        public BlobService(string connString, string? blobContainer)
        {
            Env.Load();

            var blobService = new BlobServiceClient(connString);

            // use config value if provided, else fallback to "productimages"
            var containerName = string.IsNullOrEmpty(blobContainer) ? "productimages" : blobContainer;

            _container = blobService.GetBlobContainerClient(containerName);
            _container.CreateIfNotExists();
        }

        // Upload from Stream (still useful if you want manual uploads)
        public async Task UploadAsync(string fileName, Stream stream)
        {
            var blobClient = _container.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        // Upload from IFormFile (for MVC forms)
        public async Task<string> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var blobClient = _container.GetBlobClient(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString(); // ✅ return the URL for saving in Table Storage
        }

        // List all blobs (names only)
        public IEnumerable<string> GetAllBlobs()
        {
            return _container.GetBlobs().Select(b => b.Name);
        }
    }
}
