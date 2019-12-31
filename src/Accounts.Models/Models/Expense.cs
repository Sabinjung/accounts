                              using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models
{
    public class Expense : FullAuditedEntity
    {
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public DateTime ServiceDt { get; set; }
        public int ExpensesTypeId { get; set; }
        public virtual ExpenseType ExpenseType { get; set; }
    }
}
