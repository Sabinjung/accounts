using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Timesheets.Dto
{
    [AutoMap(typeof(Expense))]
    public class ListItemDto
    {
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public DateTime ReportDt { get; set; }

        public int ExpenseTypeId { get; set; }
    }
}
