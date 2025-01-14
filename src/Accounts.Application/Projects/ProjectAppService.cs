﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using Accounts.Blob;
using Accounts.Data;
using Accounts.Data.Models;
using Accounts.Models;
using Accounts.Projects.Dto;
using Accounts.Timesheets;
using AutoMapper;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PQ;
using PQ.Pagination;
using RestSharp;
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
        private readonly IRepository<Company> CompanyRepository;
        private readonly IRepository<Timesheet> TimesheetRepository;
        private readonly IRepository<Invoice> InvoiceRepository;
        private readonly IRepository<HourLogEntry> HourlogRepository;
        private readonly IRepository<Attachment> AttachmentRepository;
        private readonly IRepository<Fieldglass> FieldglassRepository;
        private readonly QueryBuilderFactory QueryBuilder;

        private readonly IList<ProjectQueryParameters> SavedQueries;

        private readonly ITimesheetService TimesheetService;
        private readonly IConfiguration Configuration;

        public ProjectAppService(
            IRepository<Project> repository,
            IRepository<Company> companyRepository,
            IRepository<Timesheet> timesheetRepository,
            IRepository<Invoice> invoiceRepository,
            IRepository<HourLogEntry> hourlogRepository,
            IRepository<Attachment> attachmentRepository,
            IRepository<Fieldglass> fieldglassRepository,
            IAzureBlobService azureBlobService,
            QueryBuilderFactory queryBuilderFactory,
            ITimesheetService timesheetService,
            IConfiguration _Configuration,
            IMapper mapper)
       : base(repository)
        {
            AzureBlobService = azureBlobService;
            Mapper = mapper;
            CompanyRepository = companyRepository;
            TimesheetRepository = timesheetRepository;
            InvoiceRepository = invoiceRepository;
            HourlogRepository = hourlogRepository;
            AttachmentRepository = attachmentRepository;
            FieldglassRepository = fieldglassRepository;
            QueryBuilder = queryBuilderFactory;
            TimesheetService = timesheetService;
            Configuration = _Configuration;
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
            
        }

        private List<IhrmsProjectDto> GetIhrmsProjectList()
        {
            var url = Configuration.GetValue<string>("Ihrms:ProjectBaseUrl");
            RestClient client = new RestClient(url);
            RestRequest restRequest = new RestRequest(Method.GET);
            restRequest.AddHeader("Content-Type", "application/json");

            var output = client.Execute(restRequest);
            var result = JsonConvert.DeserializeObject<List<IhrmsProjectDto>>(output.Content);
            return result;
        }

        public async Task UploadAttachment(int projectId, IFormFile file, int? timehsheetId)
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
                    ContentUrl = uri.PrimaryUri.ToString(),
                    TimesheetId = timehsheetId.HasValue ? timehsheetId.Value : (int?)null
                }); ;
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

            query.WhereIf(p => !p.Keyword.IsNullOrWhiteSpace(), p => x => x.Company.DisplayName.ToUpper().Contains(p.Keyword.ToUpper()) || x.Consultant.DisplayName.ToUpper().Contains(p.Keyword.ToUpper()) || x.EndClient.ClientName.ToUpper().Contains(p.Keyword.ToUpper()));

            query.WhereIf(p => p.InvoiceCyclesId.HasValue, p => x => p.InvoiceCyclesId == x.InvoiceCycleId);

            query.WhereIf(p => p.TermId.HasValue, p => x => p.TermId == x.TermId);

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
                proj.TotalHoursBilled = Math.Round(proj.TotalHoursBilled, 2);
                proj.TotalAmountBilled = Math.Round(proj.TotalAmountBilled, 2);
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
            if (input.DiscountType == null && input.DiscountValue !=null)
                throw new UserFriendlyException("Discount Type not found.", "Please add discount type in project.");
            if (activeProjectCount > 0)
                throw new UserFriendlyException("Project cannot be created.", "Consultant has an active project.");
            
            return await base.Create(input);
        }
        public override async Task<ProjectDto> Update(ProjectDto input)
        {
            var query = Repository.GetAll().FirstOrDefault(x => x.Id == input.Id);
            if (input.EndDt.HasValue && HourlogRepository.GetAll().Any(x => x.ProjectId == input.Id && x.Day > input.EndDt))
                throw new UserFriendlyException("Cannot Update Project.", "Hours are logged beyond the end date.");
            var timesheets = TimesheetRepository.GetAll().FirstOrDefault(x=>x.ProjectId == input.Id && x.Status.Name != "Invoiced");
            if(timesheets!=null && query!=null)
                throw new UserFriendlyException("Project Edit Warning","Project has pending or approved timesheet. Please generate invoice or delete timesheet to edit project");
            if(input.DiscountType == null && input.DiscountValue != null)
                throw new UserFriendlyException("Discount Type not found.", "Please add discount type in project.");
            return await base.Update(input);
        }
        public override async Task<ProjectDto> Get(EntityDto<int> input)
        {
            var project = await Repository.GetAsync(input.Id);
            return Mapper.Map<ProjectDto>(project);
        }
        public async Task<UnassociatedProject> GetUnAssociatedHourLogReport(int projectId)
        {
            List<int> unassociatedTimesheets = TimesheetRepository.GetAllList().Where(x => x.InvoiceId == null).Select(x => x.Id).ToList();
            var unassociatedHoursProjects = await HourlogRepository.GetAllIncluding(x=>x.Project)
                                            .Where(x => x.ProjectId == projectId && (x.TimesheetId == null || unassociatedTimesheets
                                            .Contains(x.TimesheetId.Value)) && x.Day < DateTime.Now.AddDays(-45) && x.Day >= x.Project.InvoiceCycleStartDt)
                                            .GroupBy(z => new
                                            {
                                                z.Day.Date.Month,
                                                z.Day.Date.Year
                                            }).ToListAsync();

            UnassociatedProject result = new UnassociatedProject
            {
                ConsultantName = Repository.FirstOrDefault(y => y.Id == projectId).Consultant.DisplayName,
                UnassociatedProjectHourReportDtos = unassociatedHoursProjects.Select(x => new UnassociatedProjectHourlogMonthReportDto
                {
                    MonthName = x.Key.Month,
                    Year = x.Key.Year,
                    TotalHours = Math.Round((double)x.Sum(y => y.Hours),2)
                }).OrderBy(x => x.Year).ThenBy(x => x.MonthName)
            };
            return result;
        }
    }
}