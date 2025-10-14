using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace UnganaConnect.Service
{
    public class FileServices
    {
        private readonly ShareClient _shareClient;

        public FileServices(IConfiguration config)
        {
            
            var connectionString = Environment.GetEnvironmentVariable("AzureFiles");
            var shareName = Environment.GetEnvironmentVariable("ShareName");
            _shareClient = new ShareClient(connectionString, shareName);
            _shareClient.CreateIfNotExists();
        }

        // Create course folder
        public async Task<string> CreateCourseFolderAsync(string courseName)
        {
            var directory = _shareClient.GetRootDirectoryClient().GetSubdirectoryClient(courseName);
            await directory.CreateIfNotExistsAsync();
            return directory.Name;
        }

        // Upload file
        public async Task<string> UploadResourceAsync(string courseName, IFormFile file)
        {
            var directory = _shareClient.GetRootDirectoryClient().GetSubdirectoryClient(courseName);
            await directory.CreateIfNotExistsAsync();

            var fileClient = directory.GetFileClient(file.FileName);
            await fileClient.CreateAsync(file.Length);
            using var stream = file.OpenReadStream();
            await fileClient.UploadRangeAsync(
                new HttpRange(0, file.Length),
                stream);

            return fileClient.Path;
        }

        // Download file
        public async Task<Stream> GetResourceFileAsync(string courseName, string fileName)
        {
            var directory = _shareClient.GetRootDirectoryClient().GetSubdirectoryClient(courseName);
            var fileClient = directory.GetFileClient(fileName);
            var download = await fileClient.DownloadAsync();
            return download.Value.Content;
        }

        // Delete file
        public async Task DeleteResource(string courseName, string fileName)
        {
            var directory = _shareClient.GetRootDirectoryClient().GetSubdirectoryClient(courseName);
            var fileClient = directory.GetFileClient(fileName);
            await fileClient.DeleteIfExistsAsync();
        }
    }
}
