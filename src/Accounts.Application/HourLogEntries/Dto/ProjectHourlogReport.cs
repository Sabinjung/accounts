using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class ProjectHourLogReport
    {
        public ProjectHourLogReport()
        {
            DailyHourLogs = new List<DailyHourLog>();
        }

        public int ProjectId { get; internal set; }
        public string ConsultantName { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastApprovedDate { get; set; }
        public List<DailyHourLog> DailyHourLogs { get; set; }
    }

    public class DailyHourLog
    {
        public double? Hours { get; set; }
        public DateTime Day { get; set; }
        public string Status { get; set; }
    }

    public class DailyHourLogDetails
    {
        public double? Hours { get; set; }
        public DateTime Day { get; set; }
        public int ProjectId { get; set; }
        public string ConsultantName { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
    }
}