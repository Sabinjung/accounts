using Abp.Application.Services.Dto;
using System;

namespace Accounts.Projects
{
    public class CreateTimesheetInputDto
    {
        public int ProjectId { get; set; }

        public DateTime StartDt { get; set; }

        public DateTime EndDt { get; set; }

        public string NoteText { get; set; }

        public int[] AttachmentIds { get; set; }

        public int[] ExpensesIds { get; set; }
    }
}