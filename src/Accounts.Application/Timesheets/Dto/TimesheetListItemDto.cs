using Abp.AutoMapper;
using Accounts.Models;
using Accounts.Projects.Dto;
using System;

namespace Accounts.Timesheets.Dto
{
    [AutoMap(typeof(Timesheet))]
    public class TimesheetListItemDto
    {
        public int? Id { get; set; }

        public ProjectDto Project { get; set; }

        public int StatusId { get; set; }

        public DateTime StartDt { get; set; }

        public DateTime EndDt { get; set; }

        public int ProjectId { get; set; }

        public double TotalHrs { get; set; }


        public DateTime CreatedDt { get; set; }
    }
}