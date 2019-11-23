using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Timesheets.Dto
{
    public class TimesheetQueryParameters : NamedQueryParameter
    {
        public int? ProjectId { get; set; }
        public int[] StatusId { get; set; }
    }

}
