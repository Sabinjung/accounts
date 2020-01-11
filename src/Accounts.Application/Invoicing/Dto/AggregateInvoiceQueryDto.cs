using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    [AutoMap(typeof(Invoice))]
    public class AggregateInvoiceQueryDto:EntityDto
    {
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
    }
}
