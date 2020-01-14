using Accounts.Data.Dto;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts.EntityFrameworkCore.Extensions
{
    public static class IQueryableExtensions
    {

        public static IQueryable<MonthlySummary> GetMonthlyHourReport(this IQueryable<HourLogEntry> hourLogEntries)
        {
            return from mhl in hourLogEntries
                   group mhl by new { mhl.Day.Month, mhl.Day.Year } into mg
                   select new MonthlySummary
                   {
                       Month = mg.Key.Month,
                       Year = mg.Key.Year,
                       Value = mg.Sum(y => y.Hours)
                   };
        }
    }
}
