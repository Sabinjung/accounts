using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounts.Models
{
    public class HourLogEntry : FullAuditedEntity
    {
        [Required]
        public double Hours { get; set; }

        public DateTime Day { get; set; }

        public int ProjectId { get; set; }

        public int? TimesheetId { get; set; }

        public virtual Timesheet Timesheet { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public virtual Project Project { get; set; }

        public bool IsAssociatedWithTimesheet { get { return TimesheetId.HasValue; } }
    }
}