using Accounts.Data.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class HourMonthlyReport
    {
        public int ProjectId { get; set; }
        public string ConsultantName { get; set; }

        public IEnumerable<MonthlySummary> MonthlySummaries { get; set; }
    }


}
