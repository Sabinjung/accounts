using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.Models
{
    public class Company : FullAuditedEntity
    {
        public string DisplayName { get; set; }

        public string FullyQualifiedName { get; set; }

        [ConcurrencyCheck]
        public string ExternalCustomerId { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
