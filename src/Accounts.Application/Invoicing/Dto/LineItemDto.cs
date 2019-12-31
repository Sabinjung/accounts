using Abp.AutoMapper;
using Accounts.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    [AutoMap(typeof(LineItem))]
    public class LineItemDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime ReportDt { get; set; }
    }
}
