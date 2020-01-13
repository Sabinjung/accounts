using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    public class InvoiceMonthReportDto
    {
        public int Year { get; set; }
        public int MonthName { get; set; }
        public decimal MonthAmount { get; set; }
    }
}