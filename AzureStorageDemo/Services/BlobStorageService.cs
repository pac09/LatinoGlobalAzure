using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureStorageDemo.Services;
public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration config)
    {
        var connectionString = config["AzureBlobStorage:ConnectionString"];
        var containerName = config["AzureBlobStorage:ContainerName"];
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task UploadAsync(IFormFile file)
    {
        var blobClient = _containerClient.GetBlobClient(file.FileName);
        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
    }

    public async Task<Stream?> DownloadAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        if (await blobClient.ExistsAsync())
        {
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }
        return null;
    }

    public async Task<IEnumerable<string>> ListAsync()
    {
        var blobs = _containerClient.GetBlobsAsync();
        var results = new List<string>();
        await foreach (var blob in blobs)
        {
            results.Add(blob.Name);
        }
        return results;
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);
        return await blobClient.DeleteIfExistsAsync();
    }

}

