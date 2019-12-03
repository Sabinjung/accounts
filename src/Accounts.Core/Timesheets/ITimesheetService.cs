using Accounts.Data;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Timesheets
{
    public interface ITimesheetService
    {
        double CalculateTotalHours(IEnumerable<HourLogEntry> hourLogEntries);

        int BusinessDaysUntil(DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays);

        Tuple<DateTime, DateTime> CalculateTimesheetPeriod(Project project, Timesheet lastTimesheet);

        Tuple<DateTime, DateTime> CalculateTimesheetPeriod(DateTime projectStartDt, DateTime? projectEndDt, DateTime invoiceCycleStartDt, InvoiceCycles invoiceCycles, DateTime? lastTimesheetEndDt);

        bool AllTimesheetHoursEntered(DateTime startDt, DateTime endDt, IEnumerable<HourLogEntry> hourLogEntries);

    }
}
