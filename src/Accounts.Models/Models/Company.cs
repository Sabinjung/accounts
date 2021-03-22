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

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string Notes { get; set; }

        public int? TermId { get; set; }

        [ConcurrencyCheck]
        public string ExternalCustomerId { get; set; }

        public virtual ICollection<Project> Projects { get; set; }

        public virtual Term Term { get; set; }

    }
}
