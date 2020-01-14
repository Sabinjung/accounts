using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Accounts.Blob;
using Accounts.Data;
using Accounts.Models;
using Accounts.Projects.Dto;
using Accounts.Timesheets;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Projects
{
    [AbpAuthorize("Project")]
    public class ProjectAppService : AsyncCrudAppService<Project, ProjectDto>, IProjectAppService
    {
        private readonly IAzureBlobService AzureBlobService;

        private readonly IMapper Mapper;

        private readonly IRepository<Attachment> AttachmentRepository;

        private readonly QueryBuilderFactory QueryBuilder;

        private readonly IList<ProjectQueryParameters> SavedQueries;

        private readonly ITimesheetService TimesheetService;

        public ProjectAppService(
            IRepository<Project> repository,
            IRepository<Attachment> attachmentRepository,
            IAzureBlobService azureBlobService,
            QueryBuilderFactory queryBuilderFactory,
            ITimesheetService timesheetService,
            IMapper mapper)
       : base(repository)
        {
            AzureBlobService = azureBlobService;
            Mapper = mapper;
            AttachmentRepository = attachmentRepository;
            QueryBuilder = queryBuilderFactory;
            TimesheetService = timesheetService;
            SavedQueries = new List<ProjectQueryParameters>
            {
                new ProjectQueryParameters
                {
                    Name="Active",
                    IsProjectActive=true

                },
                 new ProjectQueryParameters
                {
                    Name="Inactive",
                    IsProjectActive=false

                 }
            };

            CreatePermissionName = "Project.Create";
            UpdatePermissionName = "Project.Update";
            DeletePermissionName = "Project.Delete";
        }

        public async Task UploadAttachment(int projectId, IFormFile file)
        {
            var project = await Repository.GetAsync(projectId);
            if (project != null)
            {
                var fileName = $"{project.Consultant.FirstName}_{project.Consultant.LastName}_{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";
                var uri = await AzureBlobService.UploadSingleFileAsync(file, fileName);
                project.Attachments.Add(new Attachment
                {
                    ContentType = file.ContentType,
                    Name = fileName,
                    OriginalName = file.FileName,
                    ContentUrl = uri.PrimaryUri.ToString()
                });
            }
        }

        public async Task<IEnumerable<AttachmentDto>> GetAttachments(int projectId)
        {
            var attachmentsQuery = AttachmentRepository.GetAll().Where(x => x.ProjectId == projectId && x.TimesheetId == null);
            return await Mapper.ProjectTo<AttachmentDto>(attachmentsQuery).ToListAsync();
        }

        [HttpDelete]
        public async Task DeleteAttachment(int projectId, int attachmentId)
        {
            await Repository.GetAsync(projectId);
            var attachment = await AttachmentRepository.GetAsync(attachmentId);
            await AttachmentRepository.DeleteAsync(attachment);
        }

        [HttpGet]
        public async Task<Page<ProjectListItemDto>> Search(ProjectQueryParameters queryParameter)
        {

            var query = QueryBuilder.Create<Project, ProjectQueryParameters>(Repository.GetAll());

            query.WhereIf(p => p.IsProjectActive.HasValue, p => x => p.IsProjectActive.Value
            ? x.EndDt.HasValue ? x.EndDt > DateTime.UtcNow : true
            : x.EndDt.HasValue && x.EndDt < DateTime.UtcNow);

            query.WhereIf(p => !p.Keyword.IsNullOrWhiteSpace(), p => x => x.Company.DisplayName.Contains(p.Keyword) || x.Consultant.FirstName.ToUpper().Contains(p.Keyword.ToUpper()));

            var sorts = new Sorts<Project>();

            sorts.Add(true, x => x.StartDt);

            query.ApplySorts(sorts);
            var queryParameters = SavedQueries.Select(x => Mapper.Map(queryParameter, x)).ToList();
            var result = await query.ExecuteAsync<ProjectListItemDto>(queryParameters.ToArray());

            var projects = result.Results;

            var lastTimesheetQuery = from p in Repository.GetAll()
                                     let lT = p.Timesheets.OrderByDescending(x => x.EndDt).FirstOrDefault()
                                     where projects.Any(x => x.Id == p.Id)
                                     select lT;
            var lastTimesheets = await lastTimesheetQuery.ToListAsync();

            Parallel.ForEach(result.Results, proj =>
            {
                var projectLastTimesheet = lastTimesheets.FirstOrDefault(t => t != null && t.ProjectId == proj.Id);
                var (uStartDt, uEndDt) = TimesheetService.CalculateTimesheetPeriod(proj.StartDt, proj.EndDt, proj.InvoiceCycleStartDt, (InvoiceCycles)proj.InvoiceCycleId, projectLastTimesheet?.EndDt);
                var duedays = projectLastTimesheet != null ? Math.Ceiling((DateTime.UtcNow - uStartDt).TotalDays) : Math.Ceiling((DateTime.UtcNow - uEndDt).TotalDays);
                proj.PastTimesheetDays = duedays > 0 ? duedays : 0;

            });

            return result;
        }

        public override async Task<ProjectDto> Create(ProjectDto input)
        {
            var activeProjectCount = await Repository.CountAsync(x => x.ConsultantId == input.ConsultantId && (x.EndDt.HasValue ? x.EndDt > DateTime.UtcNow : true));
            if (activeProjectCount > 0)
            {
                throw new UserFriendlyException("Consultant has active project. ");
            }
            return await base.Create(input);
        }
    }
}
