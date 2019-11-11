using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class GetAllHourLogsByProjectInput : PagedAndSortedResultRequestDto
    {

        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
    }
}
