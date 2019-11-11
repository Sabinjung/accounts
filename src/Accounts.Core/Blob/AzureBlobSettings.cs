using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Blob
{
    public class AzureBlobSettings
    {
        public string ContainerName { get; set; }
        public string StorageConnectionString { get; set; }
    }
}
