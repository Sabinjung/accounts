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
        public decimal Amount { get; set; }

        public string Comment { get; set; }

        public DateTime ServiceDt { get; set; }

        public int ExpenseTypeId { get; set; }
    }
}
