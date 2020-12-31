using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    public class AgeingReportDto
    {
        public int Key { get; set; }
        public string Days { get; set; }
        public List<Children> Children { get; set; }
    }

    [AutoMap(typeof(Invoice))]
    public class Children
    {
        public string CompanyName { get; set; }
        public string EInvoiceId { get; set; }
        public string QBOInvoiceId { get; set; }
        public string ConsultantName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Total { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
