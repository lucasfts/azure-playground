using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlobQuickstartV12
{
    class Program
    {
        static string localPath = "./data/";
        static string fileName = $"quickstart{Guid.NewGuid()}.txt";

        static async Task Main(string[] args)
        {
            var containerClient = await CreateContainer();

            string localFilePath = Path.Combine(localPath, fileName);
            await File.WriteAllTextAsync(localFilePath, "Hello World");

            var blobClient = await UploadBlobToContainer(containerClient, localFilePath);
            await ListBlobsInContainer(containerClient);

            var downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");
            await DownLoadBlobs(downloadFilePath, blobClient);
            
            await DeleteContainer(containerClient, localFilePath, downloadFilePath);
        }

        private static IConfigurationRoot GetConfiguration()
            => new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", false)
                            .Build();

        private static async Task<BlobContainerClient> CreateContainer()
        {
            var connectionString = GetConfiguration()
                .GetConnectionString("AZURE_STORAGE_CONNECTION_STRING");

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerName = $"quickstartblobs{Guid.NewGuid()}";

            BlobContainerClient containerClient = await blobServiceClient
                .CreateBlobContainerAsync(containerName);

            return containerClient;
        }

        private static async Task<BlobClient> UploadBlobToContainer(BlobContainerClient containerClient, string localFilePath)
        {
            var blobClient = containerClient.GetBlobClient(fileName);

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            await blobClient.UploadAsync(localFilePath, true);

            return blobClient;
        }

        private static async Task ListBlobsInContainer(BlobContainerClient containerClient)
        {
            Console.WriteLine("Listing blobs...");

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine($"\t{blobItem.Name}");
            }
        }

        private static async Task DownLoadBlobs(string downloadFilePath, BlobClient blobClient)
        {
            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

            await blobClient.DownloadToAsync(downloadFilePath);
        }

        private static async Task DeleteContainer(BlobContainerClient containerClient, string localFilePath, string downloadFilePath)
        {
            Console.Write("Press any key to begin clean up");
            Console.ReadLine();

            Console.WriteLine("Deleting blob container...");
            await containerClient.DeleteAsync();

            Console.WriteLine("Deleting the local source and downloaded files...");
            File.Delete(localFilePath);
            File.Delete(downloadFilePath);

            Console.WriteLine("Done");
        }
    }
}
