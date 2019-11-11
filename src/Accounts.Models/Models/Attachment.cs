using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class Attachment : FullAuditedEntity
    {
        public string Name { get; set; }

        public string OriginalName { get; set; }

        public string ContentType { get; set; }

        public int ProjectId { get; set; }

        public string ContentUrl { get; set; }

        public virtual Project Project { get; set; }
    }
}
