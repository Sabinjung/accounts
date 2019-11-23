using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Accounts.Models
{
    public class Project : FullAuditedEntity
    {
        public DateTime StartDt { get; set; }

        public DateTime? EndDt { get; set; }

        public int TermId { get; set; }

        public int InvoiceCycleId { get; set; }

        public int CompanyId { get; set; }

        public int ConsultantId { get; set; }

        public double Rate { get; set; }

        public bool IsDiscountPercentageApplied { get; set; }

        public double? DiscountPercentage { get; set; }

        public decimal? DiscountAmount { get; set; }

        public virtual Company Company { get; set; }

        public virtual Term Term { get; set; }

        public virtual Consultant Consultant { get; set; }

        public virtual InvoiceCycle InvoiceCycle { get; set; }

        public virtual ICollection<HourLogEntry> HourLogEntries { get; set; }

        public virtual ICollection<Timesheet> Timesheets { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }

    }
}
