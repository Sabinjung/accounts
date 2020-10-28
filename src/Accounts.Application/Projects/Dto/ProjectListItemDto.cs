using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.Projects.Dto
{
    [AutoMap(typeof(Project))]
    public class ProjectListItemDto : EntityDto
    {
        public int ConsultantId { get; set; }

        public string ConsultantName { get; set; }

        public string CompanyName { get; set; }

        public int CompanyId { get; set; }

        public string ClientName { get; set; }

        public int EndClientId { get; set; }

        public DateTime StartDt { get; set; }

        public DateTime? EndDt { get; set; }

        public DateTime InvoiceCycleStartDt { get; set; }

        public string InvoiceCycleName { get; set; }

        public string TermName { get; set; }

        public int InvoiceCycleId { get; set; }

        public double Rate { get; set; }

        public double PastTimesheetDays { get; set; }

        public double TotalHoursBilled { get; set; }

        public decimal TotalAmountBilled { get; set; }
    }
}
