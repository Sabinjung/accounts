using System;
using System.Collections.Generic;
using Abp.AutoMapper;
using Accounts.HourLogEntries.Dto;
using Accounts.Models;
using Accounts.Projects;
using Accounts.Projects.Dto;

namespace Accounts.Timesheets.Dto
{

    [AutoMap(typeof(Timesheet))]
    public class TimesheetDto
    {
        public int? Id { get; set; }

        public ProjectDto Project { get; set; }
        public int InvoiceCompanyId { get; set; }
        public string InvoiceCompanyName { get; set; }


        public int StatusId { get; set; }

        public DateTime StartDt { get; set; }

        public DateTime EndDt { get; set; }

        public int ProjectId { get; set; }

        public int? InvoiceId { get; set; }

        public string QBInvoiceId { get; set; }


        public double TotalHrs { get; set; }

        public string ApprovedByUserDisplayName { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? InvoiceGeneratedDate { get; set; }

        public DateTime CreatedDt { get; set; }

        public string CreatedByUserName { get; set; }

        public string ApprovedByUserName { get; set; }

        public string InvoiceGeneratedByUserName { get; set; }
        public bool IsDeletedInIntuit { get; set; }

        public IEnumerable<HourLogEntryDto> HourLogEntries { get; set; }

        public IEnumerable<AttachmentDto> Attachments { get; set; }
    }
}