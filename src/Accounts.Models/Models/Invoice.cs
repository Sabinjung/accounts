using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Accounts.Models
{
    public class Invoice : FullAuditedEntity
    {

        public int TermId { get; set; }

        public string Memo { get; set; }

        public string Description { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public double TotalHours { get; set; }

        public double Rate { get; set; }

        public decimal SubTotal { get; set; }

        public bool IsDiscountPercentageApplied { get; set; }

        public double DiscountPercentage { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal Total { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public string QBOInvoiceId { get; set; }

        public int? DocNumber { get; set; }

        public int CustomerId { get; set; }

        public int ConsultantId { get; set; }

        public virtual Company Company { get; set; }

        public virtual Consultant Consultant { get; set; }

        public virtual Term Term { get; set; }
        public int CompanyId { get; set; }

        public virtual ICollection<Attachment> Attachments { get; set; }

    }
}
