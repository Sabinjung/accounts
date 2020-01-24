using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Accounts.Models;
using Accounts.Timesheets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.HourLogEntries.EventHandlers
{
    public class HourLogEntryEventHandler : IAsyncEventHandler<EntityUpdatingEventData<HourLogEntry>>, ITransientDependency
    {
        private readonly IRepository<Timesheet> TimesheetRepository;
        private readonly ITimesheetService TimesheetService;

        public HourLogEntryEventHandler(
            IRepository<Timesheet> timesheetRepository, ITimesheetService timesheetService)
        {
            TimesheetRepository = timesheetRepository;
            TimesheetService = timesheetService;
        }

        public async Task HandleEventAsync(EntityUpdatingEventData<HourLogEntry> eventData)
        {
            var hourLogEntry = eventData.Entity;
            if (hourLogEntry.IsAssociatedWithTimesheet)
            {
                var timesheet = await TimesheetRepository.GetAsync(hourLogEntry.TimesheetId.Value);
                timesheet.TotalHrs = TimesheetService.CalculateTotalHours(timesheet.HourLogEntries);
            }
        }
    }
}