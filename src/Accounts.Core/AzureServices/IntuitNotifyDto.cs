using System;

namespace Accounts.AzureServices
{
    public class IntuitNotifyDto
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Operation { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}