using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Consultants.Dto;
using Accounts.Models;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Consultants
{
    public class ConsultantAppService : AsyncCrudAppService<Consultant, ConsultantDto>
    {
        private readonly QueryBuilderFactory QueryBuilder;
        public ConsultantAppService(IRepository<Consultant> repository, QueryBuilderFactory queryBuilderFactory)
       : base(repository)
        {
            QueryBuilder = queryBuilderFactory;
        }

        [HttpGet]
        public async Task<Page<ConsultantDto>> Search(ConsultantSearchParameters queryParameter)
        {
            var query = QueryBuilder.Create<Consultant, ConsultantSearchParameters>(Repository.GetAll());
            query.WhereIf(p => !string.IsNullOrEmpty(p.SearchText), p => x => x.FirstName.Contains(p.SearchText));
            var result = await query.ExecuteAsync<ConsultantDto>(queryParameter);
            return result;
        }

    }



}
