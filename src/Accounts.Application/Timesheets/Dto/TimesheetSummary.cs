using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Timesheets.Dto
{
    public class TimesheetSummary
    {
        public DateTime StartDt { get; set; }

        public DateTime EndDt { get; set; }

        public double? TotalHrs { get; set; }
    }
}
