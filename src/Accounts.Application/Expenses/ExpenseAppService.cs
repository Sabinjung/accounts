using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Expenses.Dto;
using Accounts.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Abp.Authorization;

namespace Accounts.Expenses
{
    [AbpAuthorize]
    public class ExpenseAppService : AsyncCrudAppService<Expense, ExpenseDto>, IExpenseAppService
    {
        private readonly QueryBuilderFactory QueryBuilder;

        public ExpenseAppService(IRepository<Expense> repository, QueryBuilderFactory queryBuilderFactory) : base(repository)
        {
            QueryBuilder = queryBuilderFactory;
            CreatePermissionName = "Expense.Create";
            UpdatePermissionName = "Expense.Update";
            DeletePermissionName = "Expense.Delete";
        }

        [HttpGet]
        public async Task<Page<ExpenseDto>> Search(ExpenseQueryParameters queryParameter)
        {
            var query = QueryBuilder.Create<Expense, ExpenseQueryParameters>(Repository.GetAll());
            query.WhereIf(x => x.TimesheetId.HasValue, c => p => p.TimesheetId == c.TimesheetId);

            var sorts = new Sorts<Expense>();
            sorts.Add(true, c => c.CreationTime);
            query.ApplySorts(sorts);
            var result = await query.ExecuteAsync<ExpenseDto>(queryParameter);
            return result;
        }
    }
}