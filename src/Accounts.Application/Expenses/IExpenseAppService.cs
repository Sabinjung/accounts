using Abp.Application.Services;
using Accounts.Expenses.Dto;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Expenses
{
    public interface IExpenseAppService : IApplicationService
    {
        Task<Page<ExpenseDto>> Search(ExpenseQueryParameters queryParameter);
    }
}