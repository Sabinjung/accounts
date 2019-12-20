using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Data;
using Accounts.Models;
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

        public DateTime StartDt { get; set; }

        public DateTime? EndDt { get; set; }

        public DateTime InvoiceCycleStartDt { get; set; }

        public int TermId { get; set; }

        public int InvoiceCycleId { get; set; }

        public DiscountType? DiscountType { get; set; }

        public decimal? DiscountValue { get; set; }

        public double Rate { get; set; }

        public double PastTimesheetDays { get; set; }

        public IsTimeSheetSubmittedState IsTimeSheetSubmitted { get; set; }
    }

    public enum IsTimeSheetSubmittedState : byte
    {
        Open = 0,
        Submitted = 1
    }
}
