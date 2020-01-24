using Abp.Application.Services;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Accounts.Models;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization;
using Accounts.Expenses.Dto;

namespace Accounts.Expenses
{
    [AbpAuthorize]
    public class ExpenseTypeAppService : AsyncCrudAppService<ExpenseType, ExpenseTypeDto>
    {
        private readonly QueryBuilderFactory QueryBuilder;

        public ExpenseTypeAppService(IRepository<ExpenseType> repository, QueryBuilderFactory queryBuilderFactory)
        : base(repository)
        {
            QueryBuilder = queryBuilderFactory;
        }

        [HttpGet]
        public async Task<Page<ExpenseTypeDto>> Search(ExpenseTypeSearchParameters queryParameter)
        {
            var query = QueryBuilder.Create<ExpenseType, ExpenseTypeSearchParameters>(Repository.GetAll());
            query.WhereIf(p => !string.IsNullOrEmpty(p.SearchText), p => x => x.Name.Contains(p.SearchText));
            var sorts = new Sorts<ExpenseType>();
            sorts.Add(true, c => c.Name);
            query.ApplySorts(sorts);
            var result = await query.ExecuteAsync<ExpenseTypeDto>(queryParameter);
            return result;
        }
    }
}