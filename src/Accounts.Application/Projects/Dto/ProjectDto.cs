using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Data;
using Accounts.Models;
using Accounts.Timesheets.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.Projects.Dto
{
    [AutoMap(typeof(Project))]
    public class ProjectDto : EntityDto
    {
        public int ConsultantId { get; set; }

        public string ConsultantName { get; set; }

        public string CompanyName { get; set; }

        public int CompanyId { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime StartDt { get; set; }

        public DateTime? EndDt { get; set; }

        public string InvoiceCycleName { get; set; }

        public DateTime InvoiceCycleStartDt { get; set; }

        public int TermId { get; set; }

        public int InvoiceCycleId { get; set; }

        public DiscountType? DiscountType { get; set; }

        public decimal? DiscountValue { get; set; }

        public double Rate { get; set; }

        public double PastTimesheetDays { get; set; }
        
        public TimesheetStatuses TimesheetStatus { get; set; }

        public double TotalHoursBilled { get; set; }

        public decimal TotalAmountBilled { get; set; }

        public TimesheetSummary UpcomingTimesheetSummary { get; set; }

        public DateTime? LastApprovedDate { get; set; }

        public DateTime? LastInvoicedDate { get; set; }
    }
}
