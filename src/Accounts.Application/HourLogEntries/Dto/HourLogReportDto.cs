using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class HourLogReportDto
    {
        public int ProjectId { get; set; }
        public string DisplayName { get; set; }
        public double? TotalInvoicedHours { get; set; }
        public double? TotalNonInvoicedHours { get; set; }

    }
}
