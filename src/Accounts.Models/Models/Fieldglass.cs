using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Data.Models
{
    public class Fieldglass:FullAuditedEntity
    {
        public int MonthNumber { get; set; }
        public string MonthName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
