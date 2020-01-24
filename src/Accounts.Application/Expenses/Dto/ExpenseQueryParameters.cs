using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Expenses.Dto
{
    public class ExpenseQueryParameters : NamedQueryParameter
    {
        public int? TimesheetId { get; set; }
    }
}