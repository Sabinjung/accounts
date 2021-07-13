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
using System.Linq.Dynamic.Core;
using System.Linq;
using Abp.UI;
using Abp.Authorization;

namespace Accounts.EndClients
{
    public class EndClientAppService : AsyncCrudAppService<EndClient, EndClientDto>
    {
        private readonly QueryBuilderFactory QueryBuilder;
        private readonly IRepository<Project> ProjectRepository;
        public EndClientAppService(IRepository<EndClient> repository, QueryBuilderFactory queryBuilderFactory, IRepository<EndClient> endClientRepository, IRepository<Project> projectRepository)
        : base(repository)
        {
            QueryBuilder = queryBuilderFactory;
            ProjectRepository = projectRepository;
            CreatePermissionName = "Endclient.Create";
            UpdatePermissionName = "Endclient.Update";
            DeletePermissionName = "Endclient.Delete";
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
        [AbpAuthorize("Endclient.Delete")]
        [HttpDelete]
        public async Task DeleteClient([FromQuery] int id, [FromBody] DeleteEndClientDto input)
        {
            var projects = ProjectRepository.GetAllList().Where(x => x.EndClientId == id).FirstOrDefault();
            if (projects != null)
                throw new UserFriendlyException("Unable to delete end client.", "The end client is associated with  project(s).");
            var endClient = await Repository.GetAsync(id);

            if (!string.IsNullOrEmpty(input.NoteText))
            {
                endClient.Notes.Add(new Note
                {
                    NoteTitle = "EndClient Deleted",
                    NoteText = input.NoteText
                });
            }
            await Repository.DeleteAsync(id);
        }

        public override async Task<EndClientDto> Create(EndClientDto input)
        {
            var existingEndClients = await Repository.CountAsync(x => x.ClientName == input.ClientName);
            if (existingEndClients > 0)
            {
                throw new UserFriendlyException("Create EndClient Failed!!","EndClient already exists. ");
            }
            return await base.Create(input);
        }

        public override async Task<EndClientDto> Update(EndClientDto input)
        {
            var existingEndClients = await Repository.CountAsync(x => x.ClientName == input.ClientName);
            if (existingEndClients > 0)
            {
                throw new UserFriendlyException("Update EndClient Failed!!","EndClient already exists. ");
            }
            return await base.Update(input);
        }
    }
}
