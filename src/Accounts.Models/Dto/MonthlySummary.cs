using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Data.Dto
{
    public class MonthlySummary
    {
        public int? ProjectId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public double Value { get; set; }
    }
}
