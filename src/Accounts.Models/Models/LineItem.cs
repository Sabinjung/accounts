using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class LineItem : FullAuditedEntity
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime ServiceDt { get; set; }
    }
}
