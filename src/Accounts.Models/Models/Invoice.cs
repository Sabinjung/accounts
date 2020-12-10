using Abp.Domain.Entities.Auditing;
using Accounts.Data;
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

        public decimal ServiceTotal { get; set; }

        public decimal SubTotal { get; set; }

        public DiscountType? DiscountType { get; set; }

        public decimal? DiscountValue { get; set; }

        public decimal Total { get; set; }

        public string QBOInvoiceId { get; set; }

        public string EInvoiceId { get; set; }

        public decimal? Balance { get; set; }

        public int ConsultantId { get; set; }

        public int ProjectId { get; set; }

        public int CompanyId { get; set; }

        public string EndClientName { get; set; }

        public virtual Project Project { get; set; }

        public virtual Company Company { get; set; }

        public virtual Consultant Consultant { get; set; }

        public virtual Term Term { get; set; }

        public virtual IEnumerable<LineItem> LineItems { get; set; }
        
        public virtual ICollection<Attachment> Attachments { get; set; }
        
        public decimal DiscountAmount { get; set; }
    }
}
