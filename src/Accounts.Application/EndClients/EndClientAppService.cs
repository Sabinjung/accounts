using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Models;
using Accounts.EndClients.Dto;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.EndClients
{
    public class EndClientAppService : AsyncCrudAppService<EndClient, EndClientDto>
    {
        private readonly QueryBuilderFactory QueryBuilder;
        public EndClientAppService(IRepository<EndClient> repository, QueryBuilderFactory queryBuilderFactory)
        : base(repository)
        {
            QueryBuilder = queryBuilderFactory;
        }

        [HttpGet]
        public async Task<Page<EndClientDto>> Search(ClientSearchParameters queryParameter)
        {
            var query = QueryBuilder.Create<EndClient, ClientSearchParameters>(Repository.GetAll());
            query.WhereIf(p => !string.IsNullOrEmpty(p.SearchText), p => x => x.ClientName.Contains(p.SearchText));
            var sorts = new Sorts<EndClient>();
            sorts.Add(true, c => c.ClientName);
            query.ApplySorts(sorts);
            var result = await query.ExecuteAsync<EndClientDto>(queryParameter);
            return result;
        }
    }
}
