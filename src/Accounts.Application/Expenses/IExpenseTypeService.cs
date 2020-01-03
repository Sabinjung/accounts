using Abp.Application.Services;
using Accounts.Expenses.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Expenses
{
    interface IExpenseTypeService : IAsyncCrudAppService<ExpenseTypeDto>
    {
    }
}
