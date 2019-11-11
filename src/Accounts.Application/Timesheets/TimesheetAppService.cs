using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Timesheets.Dto;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Accounts.HourLogEntries.Dto;
using System.Threading.Tasks;
using Accounts.Invoicing;
using Abp.Runtime.Session;
using Abp.Authorization;
using Abp.UI;
using Abp.ObjectMapping;
using Accounts.Data;
using Accounts.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Accounts.Projects.Dto;
using AutoMapper;
using PQ.Pagination;
using PQ;

namespace Accounts.Projects
{
    public class TimesheetAppService : ApplicationService, ITimesheetAppService
    {
        private readonly IRepository<Timesheet> Repository;

        private readonly IProjectRepository ProjectRepository;

        private readonly IHourLogEntryRepository HourLogEntryRepository;
        private readonly IRepository<Attachment> AttachmentRepository;

        private readonly IMapper Mapper;

        private readonly IList<TimesheetQueryParameters> SavedQueries;

        private readonly QueryBuilderFactory QueryBuilder;
        public TimesheetAppService(
            IRepository<Timesheet> repository,
            IProjectRepository projectRepository,
            IRepository<Attachment> attachmentRepository,
            IHourLogEntryRepository hourLogEntryRepository,
            IMapper mapper,
            QueryBuilderFactory queryBuilderFactory
            )

        {
            Mapper = mapper;
            Repository = repository;
            ProjectRepository = projectRepository;
            HourLogEntryRepository = hourLogEntryRepository;
            AttachmentRepository = attachmentRepository;
            QueryBuilder = queryBuilderFactory;
            SavedQueries = new List<TimesheetQueryParameters>
            {
                new TimesheetQueryParameters
                {
                    Name="Pending Approval",
                    StatusId=(int)TimesheetStatuses.Created
                },
                 new TimesheetQueryParameters
                {
                    Name="Approved",
                    StatusId=(int)TimesheetStatuses.Approved
                },
                  new TimesheetQueryParameters
                {
                    Name="Invoiced",
                    StatusId=(int)TimesheetStatuses.Invoiced
                }
            };
        }

        [AbpAuthorize("Timesheet.Approve")]
        public async Task Approve(int timesheetId)
        {
            var currentUserId = Convert.ToInt32(AbpSession.UserId);
            var timesheet = await Repository.GetAsync(timesheetId);
            if (timesheet != null)
            {
                timesheet.StatusId = (int)TimesheetStatuses.Approved;
                timesheet.ApprovedByUserId = currentUserId;
                timesheet.ApprovedDate = DateTime.UtcNow;
            }
        }


        public async Task Create(CreateTimesheetInputDto input)
        {
            var project = await ProjectRepository.GetAsync(input.ProjectId);
            var startDt = new DateTime(project.StartDt.Year, project.StartDt.Month, 1);
            var endDt = DateTime.Now.AddDays(2);

            var hourLogentries = await HourLogEntryRepository.GetHourLogEntriesByProjectIdAsync(project.Id, startDt, endDt).ToListAsync();

            var attachments = await AttachmentRepository.GetAll().Where(a => input.AttachmentIds.Any(x => x == a.Id)).ToListAsync();
            if (project != null)
            {
                var newTimesheet = new Timesheet
                {
                    ProjectId = project.Id,
                    StatusId = (int)TimesheetStatuses.Created,
                    HourLogEntries = hourLogentries,
                    Attachments = attachments,
                    StartDt = startDt,
                    EndDt = endDt,
                };

                if (!string.IsNullOrEmpty(input.NoteText))
                {
                    newTimesheet.Notes = new List<Note>
                    {
                        new Note{ NoteText=input.NoteText}
                    };
                }

                Repository.Insert(newTimesheet);
            }
        }

        public async Task<TimesheetDto> GetUpcomingTimesheetInfo(int projectId)
        {
            var project = await ProjectRepository.GetAsync(projectId);
            var timesheet = await Repository.GetAll().Where(x => x.ProjectId == projectId).OrderByDescending(x => x.CreationTime).FirstOrDefaultAsync();
            var timesheetInfo = new TimesheetDto();
            timesheetInfo.ProjectId = projectId;
            if (project != null)
            {
                timesheetInfo.Project = Mapper.Map<ProjectDto>(project);
                timesheetInfo.StartDt = new DateTime(project.StartDt.Year, project.StartDt.Month, 1);
                timesheetInfo.EndDt = timesheetInfo.StartDt.AddMonths(1).AddDays(-1);

                var hourLogEntries = await HourLogEntryRepository.GetHourLogEntriesByProjectIdAsync(project.Id, timesheetInfo.StartDt, timesheetInfo.EndDt).ToListAsync();
                timesheetInfo.HourLogEntries = Mapper.Map<IEnumerable<HourLogEntryDto>>(hourLogEntries);
                timesheetInfo.TotalHrs = timesheetInfo.HourLogEntries.Sum(x => x.Hours);
            }
            return timesheetInfo;
        }


        [HttpGet]
        public async Task<TimesheetDto> Detail(int timesheetId)
        {
            var timesheet = await Mapper.ProjectTo<TimesheetDto>(Repository.GetAll().Where(x => x.Id == timesheetId)).FirstOrDefaultAsync();
            return timesheet;
        }

        public async Task<Page<TimesheetListItemDto>> GetTimesheets(TimesheetQueryParameters queryParameter)
        {
            var query = QueryBuilder.Create<Timesheet, TimesheetQueryParameters>(Repository.GetAll());
            query.WhereIf(p => p.ProjectId.HasValue, p => x => x.ProjectId == p.ProjectId);
            query.WhereIf(p => p.StatusId.HasValue, p => x => x.StatusId == p.StatusId);
            var queryParameters = SavedQueries.Select(x => Mapper.Map(queryParameter, x)).ToList();
            var result = await query.ExecuteAsync<TimesheetListItemDto>(queryParameters.ToArray());
            return result;
        }


        public IEnumerable<TimesheetQueryParameters> GetSavedQueries()
        {
            return SavedQueries;
        }



    }
}
