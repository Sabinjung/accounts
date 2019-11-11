using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.HourLogEntries.Dto
{
    [AutoMapTo(typeof(HourLogEntry))]
    public class AddUpdateHourLogEntryDto
    {

    }
}
