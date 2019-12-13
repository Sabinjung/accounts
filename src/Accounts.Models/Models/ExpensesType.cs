using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class ExpensesType : FullAuditedEntity
    {
        public string ExpensesName { get; set; }
        public string Description { get; set; }
    }
}
