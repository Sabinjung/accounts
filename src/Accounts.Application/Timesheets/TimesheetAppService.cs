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
using Accounts.Timesheets;
using MoreLinq;

namespace Accounts.Projects
{
    [AbpAuthorize("Timesheet")]
    public class TimesheetAppService : ApplicationService, ITimesheetAppService
    {
        private readonly IRepository<Timesheet> Repository;

        private readonly IProjectRepository ProjectRepository;

        private readonly IHourLogEntryRepository HourLogEntryRepository;

        private readonly IRepository<Attachment> AttachmentRepository;

        private readonly IMapper Mapper;

        private readonly IList<TimesheetQueryParameters> SavedQueries;

        private readonly QueryBuilderFactory QueryBuilder;

        private readonly ITimesheetService TimesheetService;

        private readonly IRepository<Models.Expenses> ExpensesRepository;

        public TimesheetAppService(
            IRepository<Timesheet> repository,
            IProjectRepository projectRepository,
            IRepository<Attachment> attachmentRepository,
            IHourLogEntryRepository hourLogEntryRepository,
            IMapper mapper,
            QueryBuilderFactory queryBuilderFactory,
            ITimesheetService timesheetService,
            IRepository<Models.Expenses> expensesRepository
            )

        {
            Mapper = mapper;
            Repository = repository;
            ProjectRepository = projectRepository;
            HourLogEntryRepository = hourLogEntryRepository;
            AttachmentRepository = attachmentRepository;
            QueryBuilder = queryBuilderFactory;
            ExpensesRepository = expensesRepository;
            SavedQueries = new List<TimesheetQueryParameters>
            {
                new TimesheetQueryParameters
                {
                    Name="Pending Apprv",
                    StatusId=new int[]{(int)TimesheetStatuses.Created }
                },
                 new TimesheetQueryParameters
                {
                    Name="Approved",
                    StatusId=new int[]{(int)TimesheetStatuses.Approved }
                },
                  new TimesheetQueryParameters
                {
                    Name="Inv. Gen",
                    StatusId=new int[]{(int)TimesheetStatuses.InvoiceGenerated, }
                },
                   new TimesheetQueryParameters
                {
                    Name="Invoiced",
                    StatusId=new int[]{ (int) TimesheetStatuses.Invoiced }
                }
            };
            TimesheetService = timesheetService;
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


        [AbpAuthorize("Timesheet.Create")]
        public async Task Create(CreateTimesheetInputDto input)
        {
            // Gather required information
            var project = await ProjectRepository.GetAsync(input.ProjectId);
            var lastTimesheet = await Repository.GetAll().Where(x => x.ProjectId == input.ProjectId).OrderByDescending(x => x.EndDt).FirstOrDefaultAsync();
            var (startDt, endDt) = TimesheetService.CalculateTimesheetPeriod(project, lastTimesheet);
            var hourLogentries = await HourLogEntryRepository.GetHourLogEntriesByProjectIdAsync(project.Id, startDt, endDt).ToListAsync();
            var attachments = await AttachmentRepository.GetAll().Where(a => input.AttachmentIds.Any(x => x == a.Id)).ToListAsync();
            var expenses = await ExpensesRepository.GetAll().Where(e => input.Expenses.Any(y => y == e.Id)).ToListAsync();

            var distinctHourLogEntries = hourLogentries.DistinctBy(x => x.Day).ToList();
            // Construct new Timesheet
            var newTimesheet = new Timesheet
            {
                ProjectId = project.Id,
                StatusId = (int)TimesheetStatuses.Created,
                HourLogEntries = distinctHourLogEntries,
                Attachments = attachments,
                Expenses = expenses,
                StartDt = startDt,
                EndDt = endDt,
                TotalHrs = TimesheetService.CalculateTotalHours(distinctHourLogEntries)
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

        public async Task<TimesheetDto> GetUpcomingTimesheetInfo(int projectId)
        {
            return await CreateNextTimesheet(projectId);
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
            query.WhereIf(p => p.StatusId != null && p.StatusId.Length > 0, p => x => p.StatusId.Contains(x.StatusId));

            var sorts = new Sorts<Timesheet>();
            sorts.Add(true, t => t.CreationTime);
            query.ApplySorts(sorts);

            var queryParameters = SavedQueries.Select(x => Mapper.Map(queryParameter, x)).ToList();
            var result = await query.ExecuteAsync<TimesheetListItemDto>(queryParameters.ToArray());
            return result;
        }


        public IEnumerable<TimesheetQueryParameters> GetSavedQueries()
        {
            return SavedQueries;
        }


        private async Task<TimesheetDto> CreateNextTimesheet(int projectId)
        {
            var timesheetInfo = new TimesheetDto();
            var project = await ProjectRepository.GetAsync(projectId);
            var lastTimesheet = await Repository.GetAll().Where(x => x.ProjectId == projectId).OrderByDescending(x => x.EndDt).FirstOrDefaultAsync();
            timesheetInfo.ProjectId = project.Id;
            timesheetInfo.Project = Mapper.Map<ProjectDto>(project);
            var (startDt, endDt) = TimesheetService.CalculateTimesheetPeriod(project, lastTimesheet);
            timesheetInfo.StartDt = startDt;
            timesheetInfo.EndDt = endDt;

            // Fill in Timesheet Hour Log Entries
            var hourLogEntries = await HourLogEntryRepository.GetHourLogEntriesByProjectIdAsync(project.Id, timesheetInfo.StartDt, timesheetInfo.EndDt).ToListAsync();

            if (!TimesheetService.AllTimesheetHoursEntered(project.StartDt > startDt ? project.StartDt : startDt, timesheetInfo.EndDt, hourLogEntries))
            {
                throw new UserFriendlyException($"Please enter all billable hours between {timesheetInfo.StartDt.ToShortDateString()}-{timesheetInfo.EndDt.ToShortDateString()}");
            }

            timesheetInfo.HourLogEntries = Mapper.Map<IEnumerable<HourLogEntryDto>>(hourLogEntries);
            timesheetInfo.TotalHrs = timesheetInfo.HourLogEntries.Sum(x => x.Hours);
            return timesheetInfo;
        }



    }
}
