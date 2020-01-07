using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class HourLogQueryParameter
    {
        public DateTime startDt { get; set; }
        public DateTime endDt { get; set; }
        public DateTime endMonth { get; set; }
        public int? projectId { get; set; }
        public int? consultantId { get; set; }
    }
}
