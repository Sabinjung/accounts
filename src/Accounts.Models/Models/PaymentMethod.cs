using Abp.Domain.Entities.Auditing;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class PaymentMethod : FullAuditedEntity
    {
        public string Name { get; set; }

        public string ExternalPaymentId { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
