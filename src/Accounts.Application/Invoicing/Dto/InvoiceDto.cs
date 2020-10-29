using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Data;
using Accounts.Models;
using Accounts.Projects;
using Accounts.Timesheets.Dto;
using System;
using System.Collections.Generic;

namespace Accounts.Invoicing.Dto
{
    [AutoMap(typeof(Invoice))]
    public class InvoiceDto : EntityDto
    {
        public int? TimesheetId { get; set; }
        public string Description { get; set; }

        public DateTime InvoiceDate { get; set; }

        public DateTime DueDate { get; set; }

        public double TotalHours { get; set; }

        public double Rate { get; set; }

        public decimal ServiceTotal { get; set; }

        public decimal SubTotal { get; set; }

        public DiscountType? DiscountType { get; set; }

        public double? DiscountValue { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal Total { get; set; }

        public string ConsultantName { get; set; }

        public string CompanyName { get; set; }

        public string ClientName { get; set; }

        public string TermName { get; set; }

        public string CompanyEmail { get; set; }

        public string QBOInvoiceId { get; set; }

        public bool IsSendMail { get; set; }

        public List<LineItemDto> LineItems { get; set; }

        public IEnumerable<AttachmentDto> Attachments { get; set; }
    }
}