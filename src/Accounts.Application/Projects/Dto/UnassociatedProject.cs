using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Projects.Dto
{
    public class UnassociatedProject
    {
        public string ConsultantName { get; set; }
        public IEnumerable<UnassociatedProjectHourlogMonthReportDto> UnassociatedProjectHourReportDtos { get; set; }
    }
    public class UnassociatedProjectHourlogMonthReportDto
    {
        public int Year { get; set; }
        public int MonthName { get; set; }
        public double TotalHours { get; set; }
    }
}
