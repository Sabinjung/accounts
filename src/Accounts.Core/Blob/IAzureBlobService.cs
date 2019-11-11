using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Blob
{
    public interface IAzureBlobService
    {
        Task<IEnumerable<Uri>> ListAsync();
        Task UploadAsync(IFormFileCollection files);
        Task<StorageUri> UploadSingleFileAsync(IFormFile file, string fileName = null);
        Task DeleteAsync(string fileUri);
        Task DeleteAllAsync();
        Task<CloudBlockBlob> DownloadFilesAsync(string fileName);
    }
}
