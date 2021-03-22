using Abp.Domain.Entities.Auditing;
using Accounts.Data;
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

        public DateTime? InvoiceCycleStartDt { get; set; }

        public int TermId { get; set; }

        public int InvoiceCycleId { get; set; }

        public int? EndClientId { get; set; }

        public int CompanyId { get; set; }

        public int ConsultantId { get; set; }

        public double Rate { get; set; }

        public bool IsSendMail { get; set; }
        public string Memo { get; set; }

        public DiscountType? DiscountType { get; set; }

        public decimal? DiscountValue { get; set; }

        public virtual EndClient EndClient { get; set; }
        public virtual Company Company { get; set; }

        public virtual Term Term { get; set; }

        public virtual Consultant Consultant { get; set; }

        public virtual InvoiceCycle InvoiceCycle { get; set; }

        public virtual ICollection<HourLogEntry> HourLogEntries { get; set; }

        public virtual ICollection<Timesheet> Timesheets { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }

    }
}

namespace Accounts.Data
{
    public enum DiscountType
    {
        Percentage = 1,
        Value

    }
}