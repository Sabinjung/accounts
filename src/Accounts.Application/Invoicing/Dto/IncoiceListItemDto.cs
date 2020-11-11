using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    [AutoMap(typeof(Invoice))]
    public class IncoiceListItemDto : EntityDto
    {
        public string QBOInvoiceId { get; set; }
        public string CompanyName { get; set; }
        public string EndClientName { get; set; }
        public string ConsultantName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public decimal? Balance { get; set; }
    }
}