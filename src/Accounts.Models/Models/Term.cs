using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class Term : FullAuditedEntity
    {
        public string Name { get; set; }

        public int DueDays { get; set; }

        public int DiscountDays { get; set; }

        public string ExternalTermId { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
