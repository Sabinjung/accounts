using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Projects.Dto;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    public class ProjectHourLogEntryDto : EntityDto
    {
        public ProjectDto Project { get; set; }

        public IEnumerable<HourLogEntryDto> HourLogEntries { get; set; }
    }

    public class InvoicedHourLogEntryDto
    {
        public List<HourLogDto> HourLogEntries { get; set; }
    }

    public class HourLogDto
    {
        public int ProjectId { get; set; }
        public DateTime Day { get; set; }
        public double? Hours { get; set; }
    }

    [AutoMap(typeof(HourLogEntry))]
    public class HourLogEntryDto : EntityDto
    {
        public int? ProjectId { get; set; }

        public DateTime Day { get; set; }

        public double? Hours { get; set; }

        public bool IsAssociatedWithTimesheet { get; set; }

        public int TimesheetStatusesId { get; set; }
    }
}