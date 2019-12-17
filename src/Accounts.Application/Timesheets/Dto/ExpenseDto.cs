using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Timesheets.Dto
{
    [AutoMap(typeof(Expense))]
    public class ExpenseDto
    {
        public int Value { get; set; }

        public string Comment { get; set; }

        public DateTime ReportDt { get; set; }

        public int ExpensesTypeId { get; set; }
    }
}
