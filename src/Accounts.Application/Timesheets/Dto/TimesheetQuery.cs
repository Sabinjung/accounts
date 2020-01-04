using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Timesheets.Dto
{
    public class TimesheetQueryParameters : NamedQueryParameter
    {
        public int? ConsultantId { get; set; }
        public int? CompanyId { get; set; }
        public int? ProjectId { get; set; }
        public int[] StatusId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

}
