using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Expenses.Dto
{
    [AutoMap(typeof(ExpenseType))]
    public class ExpensesTypeDto : EntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
