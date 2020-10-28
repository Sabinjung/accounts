using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class EndClient : FullAuditedEntity
    {
        public string ClientName { get; set; }
    }
}
