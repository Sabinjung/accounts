using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Expenses.Dto
{
    [AutoMap(typeof(Expense))]
    public class ExpenseDto : EntityDto
    {
        public decimal Amount { get; set; }
        public string Comment { get; set; }
        public DateTime? ReportDt { get; set; }
        public int TimesheetId { get; set; }
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
    }
}