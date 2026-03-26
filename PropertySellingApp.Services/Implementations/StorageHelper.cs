using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using OpenAI.Containers;
using PropertySellingApp.Services.Interfaces;

namespace PropertySellingApp.Services.Implementations
{
   

    public class StorageHelper : IStorage
    {
        private readonly BlobContainerClient _containerClient;

        private readonly string connectionString;
        private readonly string containerName;

        public StorageHelper(IConfiguration configuration)
        {
             connectionString = configuration["BlobStoragev2:ConnectionStringv2"];
             containerName = configuration["BlobStoragev2:ContainerNamev2"];

            _containerClient = new BlobContainerClient(connectionString, containerName);
            //_containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }


        public string GenerateSasUrl(string fileName, int validMinutes = 2)
        {
            //var blobClient = new BlobClient(connectionString, containerName, fileName);

            var blobClient = _containerClient.GetBlobClient(fileName);


            // Check if blob exists
            if (!blobClient.Exists())
                throw new Exception("Blob not found");

            // Generate SAS token
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = fileName,
                Resource = "b", // b = blob
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(validMinutes)
            };

            // Set permissions (Read only in this case)
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Generate SAS URI
            var sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri.ToString();
        }




        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
            
                return blobClient.Uri.ToString(); // ✅ Return public URL
        }

        public async Task<Stream> GetAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }

        public async Task<bool> DeleteAsync(string fileName)
        {
            var blobClient = _containerClient.GetBlobClient(fileName);
            return await blobClient.DeleteIfExistsAsync();
        }
    }

}
