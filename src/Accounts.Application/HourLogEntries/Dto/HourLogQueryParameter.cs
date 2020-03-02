using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class HourLogQueryParameter : NamedQueryParameter
    {
        public DateTime? StartDt { get; set; }
        public DateTime? EndDt { get; set; }
        public int? ProjectId { get; set; }
    }
}
