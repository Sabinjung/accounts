using Abp.Domain.Entities.Auditing;
using Accounts.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Accounts.Models
{
    public class Timesheet : FullAuditedEntity
    {
        public int ProjectId { get; set; }

        public DateTime StartDt { get; set; }

        public DateTime EndDt { get; set; }

        public int StatusId { get; set; }
        
            // Approval
        public long? ApprovedByUserId { get; set; }

        public DateTime? ApprovedDate { get; set; }

        // Invoice
        public int? InvoiceId { get; set; }

        public long? InvoiceGeneratedByUserId { get; set; }

        public DateTime? InvoiceGeneratedDate { get; set; }

        public double TotalHrs { get; set; }

        [ForeignKey(nameof(StatusId))]
        public virtual TimesheetStatus Status { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public virtual Invoice Invoice { get; set; }

        public virtual Project Project { get; set; }

        public virtual ICollection<Expenses> Expenses { get; set; }
        public virtual ICollection<HourLogEntry> HourLogEntries { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual User CreatorUser { get; set; }

        public virtual User ApprovedByUser { get; set; }

        public virtual User InvoiceGeneratedByUser { get; set; }


    }
}
