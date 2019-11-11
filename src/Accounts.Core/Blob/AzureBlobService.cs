using Abp.Dependency;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Blob
{
    public class AzureBlobService : IAzureBlobService, ISingletonDependency
    {
        private readonly IAzureBlobConnectionFactory _azureBlobConnectionFactory;


        public AzureBlobService(IAzureBlobConnectionFactory azureBlobConnectionFactory
            )
        {
            _azureBlobConnectionFactory = azureBlobConnectionFactory;
        }

        public async Task DeleteAllAsync()
        {
            var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();

            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var response = await blobContainer.ListBlobsSegmentedAsync(blobContinuationToken);
                foreach (IListBlobItem blob in response.Results)
                {
                    if (blob.GetType() == typeof(CloudBlockBlob))
                        await ((CloudBlockBlob)blob).DeleteIfExistsAsync();
                }
                blobContinuationToken = response.ContinuationToken;
            } while (blobContinuationToken != null);
        }

        public async Task DeleteAsync(string fileUri)
        {
            var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();

            Uri uri = new Uri(fileUri);
            string filename = Path.GetFileName(uri.LocalPath);

            var blob = blobContainer.GetBlockBlobReference(filename);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<IEnumerable<Uri>> ListAsync()
        {
            var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();
            var allBlobs = new List<Uri>();
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var response = await blobContainer.ListBlobsSegmentedAsync(blobContinuationToken);
                foreach (IListBlobItem blob in response.Results)
                {
                    if (blob.GetType() == typeof(CloudBlockBlob))
                        allBlobs.Add(blob.Uri);
                }
                blobContinuationToken = response.ContinuationToken;
            } while (blobContinuationToken != null);
            return allBlobs;
        }

        public async Task UploadAsync(IFormFileCollection files)
        {
            var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();

            for (int i = 0; i < files.Count; i++)
            {
                var blob = blobContainer.GetBlockBlobReference(GetRandomBlobName(files[i].FileName));
                using (var stream = files[i].OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);

                }
            }
        }

        public async Task<StorageUri> UploadSingleFileAsync(IFormFile file, string fileName = null)
        {
            fileName = string.IsNullOrEmpty(fileName) ? GetRandomBlobName(file.FileName) : fileName;
            var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();
            var blob = blobContainer.GetBlockBlobReference(fileName);
            using (var stream = file.OpenReadStream())
            {
                await blob.UploadFromStreamAsync(stream);
            }
            return blob.StorageUri;
        }

        public async Task<CloudBlockBlob> DownloadFilesAsync(string fileName)
        {
            var blobContainer = await _azureBlobConnectionFactory.GetBlobContainer();
            var blob = blobContainer.GetBlockBlobReference(fileName);

            //Console.WriteLine("Downloading blob to {0}", destinationFile);

            if (await blob.ExistsAsync())
            {
                MemoryStream ms = new MemoryStream();
                await blob.DownloadToStreamAsync(ms);
            }
            return blob;

        }

        private string GetRandomBlobName(string filename)
        {
            string ext = Path.GetExtension(filename);
            return string.Format("{0:10}_{1}{2}", DateTime.Now.Ticks, Guid.NewGuid(), ext);
        }
    }
}
