using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Accounts.Companies;
using Accounts.Consultants.Dto;
using Accounts.Models;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Consultants
{
    [AbpAuthorize]
    public class ConsultantAppService : AsyncCrudAppService<Consultant, ConsultantDto, int, PagedCompanyResultRequestDto>
    {
        private readonly QueryBuilderFactory QueryBuilder;
        public ConsultantAppService(IRepository<Consultant> repository, QueryBuilderFactory queryBuilderFactory)
       : base(repository)
        {
            QueryBuilder = queryBuilderFactory;
            CreatePermissionName = "Consultant.Create";
            UpdatePermissionName = "Consultant.Update";
            DeletePermissionName = "Consultant.Delete";
        }

        [HttpGet]
        public async Task<Page<ConsultantDto>> Search(ConsultantSearchParameters queryParameter)
        {
            var query = QueryBuilder.Create<Consultant, ConsultantSearchParameters>(Repository.GetAll());
            query.WhereIf(p => !string.IsNullOrEmpty(p.SearchText), p => x => x.FirstName.Contains(p.SearchText));
            var result = await query.ExecuteAsync<ConsultantDto>(queryParameter);
            return result;
        }
        protected override IQueryable<Consultant> CreateFilteredQuery(PagedCompanyResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.FirstName.Contains(input.Keyword) || x.LastName.Contains(input.Keyword));
        }

    }



}
