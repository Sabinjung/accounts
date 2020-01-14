using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class HourMonthlyReport
    {

        public string ConsultantName { get; set; }

        public IEnumerable<MonthlySummary> MonthlySummaries { get; set; }
    }

    public class MonthlySummary
    {
        public int? ProjectId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public double Value { get; set; }
    }
}
