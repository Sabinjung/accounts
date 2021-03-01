using Abp.Domain.Entities.Auditing;
using Accounts.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class Config : FullAuditedEntity
    {
        public string Data { get; set; }
        public int ConfigTypeId { get; set; }

        public virtual ConfigType ConfigType { get; set; }
    }
}
