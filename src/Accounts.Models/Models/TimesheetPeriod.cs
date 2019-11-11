using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class TimesheetPeriod : FullAuditedEntity
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int InvoiceCycleId { get; set; }
    }
}
