using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.EntityFrameworkCore.Repositories
{
    public class ProjectHourLogsQueryParameter:NamedQueryParameter
    {
        public string SearchText { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
        public int? ProjectId { get; set; }
        public int? ConsultantId { get; set; }
    }
}
