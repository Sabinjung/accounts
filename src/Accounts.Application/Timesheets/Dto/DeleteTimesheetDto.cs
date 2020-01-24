using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace Accounts.Timesheets.Dto
{
    public class DeleteTimesheetDto
    {
        [Required]
        public int TimesheetId { get; set; }

        public string NoteText { get; set; }
    }
}
