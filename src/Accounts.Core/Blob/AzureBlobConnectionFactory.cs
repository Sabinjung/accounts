using Abp.Dependency;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Blob
{
    public class AzureBlobConnectionFactory : IAzureBlobConnectionFactory, ISingletonDependency
    {

        private readonly IOptions<AzureBlobSettings> azureBlobSettings;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;

        public AzureBlobConnectionFactory(IOptions<AzureBlobSettings> azureBlobSettings)
        {

            this.azureBlobSettings = azureBlobSettings;
        }

        public async Task<CloudBlobContainer> GetBlobContainer()
        {
            if (_blobContainer != null)
                return _blobContainer;

            var containerName = this.azureBlobSettings.Value.ContainerName;
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("Configuration must contain ContainerName");
            }

            var blobClient = GetClient();

            _blobContainer = blobClient.GetContainerReference(containerName);
            await _blobContainer.CreateIfNotExistsAsync();

            //if (await _blobContainer.CreateIfNotExistsAsync())
            //{
            //    await _blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            //}
            if (_blobContainer == null)
            {
                throw new ArgumentException("Blob container is null");
            }
            return _blobContainer;
        }

        private CloudBlobClient GetClient()
        {
            if (_blobClient != null)
                return _blobClient;

            var storageConnectionString = this.azureBlobSettings.Value.StorageConnectionString;
            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                throw new ArgumentException("Configuration must contain StorageConnectionString");
            }

            if (!CloudStorageAccount.TryParse(storageConnectionString, out CloudStorageAccount storageAccount))
            {
                throw new Exception("Could not create storage account with StorageConnectionString configuration");
            }

            _blobClient = storageAccount.CreateCloudBlobClient();
            return _blobClient;
        }
    }
}
