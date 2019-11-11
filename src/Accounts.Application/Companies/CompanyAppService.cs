using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Companies.Dto;
using Accounts.Models;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Companies
{
    public class CompanyAppService : AsyncCrudAppService<Company, CompanyDto>
    {
        private readonly QueryBuilderFactory QueryBuilder;
        public CompanyAppService(IRepository<Company> repository, QueryBuilderFactory queryBuilderFactory)
       : base(repository)
        {
            QueryBuilder = queryBuilderFactory;
        }

        [HttpGet]
        public async Task<Page<CompanyDto>> Search(CompanySearchParameters queryParameter)
        {
            var query = QueryBuilder.Create<Company, CompanySearchParameters>(Repository.GetAll());
            query.WhereIf(p => !string.IsNullOrEmpty(p.SearchText), p => x => x.DisplayName.Contains(p.SearchText));
            var result = await query.ExecuteAsync<CompanyDto>(queryParameter);
            return result;
        }

    }



}
