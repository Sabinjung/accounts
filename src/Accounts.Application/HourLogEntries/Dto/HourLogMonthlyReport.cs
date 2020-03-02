using Accounts.Data.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class HourMonthlyReport
    {
        public int ProjectId { get; internal set; }

        public string ConsultantName { get; set; }

        public string  CompanyName { get; set; }
        
        public bool IsProjectActive { get; set; } 
        
        public IEnumerable<MonthlySummary> MonthlySummaries { get; set; }
    }
}
