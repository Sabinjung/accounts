using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Timesheets.Dto
{ 
    public class DeleteTimesheetDto
    {
        public string NoteText { get; set; }
    }
}
