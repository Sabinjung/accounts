using Abp.Application.Services;
using Abp.Authorization;
using Accounts.Configs.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Accounts.Models;
using Abp.Domain.Repositories;
using PQ.Pagination;
using System.Threading.Tasks;
using PQ;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Accounts.Data;

namespace Accounts.Configs
{
    [AbpAuthorize]
    public class ConfigAppService : AsyncCrudAppService<Config, ConfigDto>, IConfigAppService
    {
        public ConfigAppService(IRepository<Config> repository, QueryBuilderFactory queryBuilderFactory) : base(repository)
        {
            QueryBuilder = queryBuilderFactory;
        }

        private readonly QueryBuilderFactory QueryBuilder;

        [HttpGet]
        public async Task<Page<ConfigDto>> Search(ConfigSearchParameters queryParameter)
        {
            var query = QueryBuilder.Create<Config, ConfigSearchParameters>(Repository.GetAll().Where(x=>x.ConfigTypeId != (int)ConfigTypes.BaseUrl));
            query.WhereIf(p => !string.IsNullOrEmpty(p.ConfigType), p => x => x.ConfigType.Name.Contains(p.ConfigType));
            query.WhereIf(p => !string.IsNullOrEmpty(p.Email), p => x => x.Data.Contains(p.Email));
            

            var sorts = new Sorts<Config>();
            sorts.Add(true, c => c.ConfigType.Name);
            query.ApplySorts(sorts);
            var result = await query.ExecuteAsync<ConfigDto>(queryParameter);
            return result;
        }
       
    }
}
