using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    [AutoMap(typeof(Invoice))]
    public class IncoiceListItemDto : EntityDto
    {
        public string QBOInvoiceId { get; set; }
        public string EInvoiceId { get; set; }
        public string CompanyName { get; set; }
        public List<string> CompanyEmail { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string EndClientName { get; set; }
        public string ConsultantName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public decimal? Balance { get; set; }
        public int? DueDays { get; set; }
        public bool IsDeletedInIntuit { get; set; }

    }

    public class InvoiceListItemDto
    {
        public DateTime? LastUpdated { get; set; }
        public Page<IncoiceListItemDto> ListItemDto { get; set; }
    }
}