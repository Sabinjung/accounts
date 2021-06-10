using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class IntuitWebhookLog : FullAuditedEntity
    {
        public string TranId { get; set; }
        public string Name { get; set; }
        public string Operation { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
