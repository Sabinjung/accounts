using Abp.Application.Services;
using Abp.Application.Services.Dto;

using Abp.Domain.Repositories;
using Abp.Extensions;
using Accounts.Companies.Dto;
using Accounts.Models;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using Abp.Authorization;

namespace Accounts.Companies
{
    [AbpAuthorize("Company")]
    public class CompanyAppService : AsyncCrudAppService<Company, CompanyDto, int, PagedCompanyResultRequestDto>
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




        protected override IQueryable<Company> CreateFilteredQuery(PagedCompanyResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.DisplayName.Contains(input.Keyword));
        }

    }



}
