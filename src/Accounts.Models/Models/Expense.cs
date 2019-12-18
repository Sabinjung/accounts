using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class Expense : FullAuditedEntity
    {
        public int Value { get; set; }
        public string Comment { get; set; }
        public DateTime ReportDt { get; set; }
        public int ExpensesTypeId { get; set; }
        public virtual ExpenseType ExpenseType { get; set; }
    }
}
