using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class UnassociatedProjectHourlogReportDto
    {
        public string ConsultantName { get; set; }
        public IEnumerable<UnassociatedHourlogMonthReportDto> UnassociatedHourReportDtos { get; set; }
    }
    public class UnassociatedHourlogMonthReportDto
    {
        public int Year { get; set; }
        public IEnumerable<DailyHour> Days { get; set; }
        public int MonthName { get; set; }
        public double TotalHours { get; set; }
    }

    public class DailyHour
    {
        public DateTime Day { get; set; }
        public double? Hour { get; set; }
    }
}
