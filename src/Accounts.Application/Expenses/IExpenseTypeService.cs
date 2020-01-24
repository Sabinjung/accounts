using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Accounts.Expenses.Dto;

namespace Accounts.Expenses
{
    internal interface IExpenseTypeService : IAsyncCrudAppService<ExpenseTypeDto>
    {
    }
}