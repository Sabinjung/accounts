using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class ExpenseType : FullAuditedEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
