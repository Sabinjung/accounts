using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Timesheets.Dto;
using Accounts.Models;
using System;
using System.Collections.Generic;
using Accounts.HourLogEntries.Dto;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.UI;
using Accounts.Data;
using Accounts.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Accounts.Projects.Dto;
using PQ.Pagination;
using PQ;
using PQ.Extensions;
using Accounts.Timesheets;
using MoreLinq;
using Accounts.Data.Dto;
using Accounts.EntityFrameworkCore.Extensions;

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

        public TimesheetAppService(
            IRepository<Timesheet> repository,
            IProjectRepository projectRepository,
            IRepository<Attachment> attachmentRepository,
            IHourLogEntryRepository hourLogEntryRepository,
            IMapper mapper,
            QueryBuilderFactory queryBuilderFactory,
            ITimesheetService timesheetService

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
            var distinctHourLogEntries = hourLogentries.DistinctBy(x => x.Day).ToList();


            if ((input.StartDt.Date >= startDt.Date && input.StartDt.Date <= endDt.Date) &&
               (input.EndDt.Date >= startDt.Date && input.EndDt.Date <= endDt.Date) &&
               (input.StartDt.Date < endDt.Date && input.EndDt.Date > startDt.Date))
            {
                startDt = input.StartDt;
                endDt = input.EndDt;
            }


            // Construct new Timesheet
            var newTimesheet = new Timesheet
            {
                ProjectId = project.Id,
                StatusId = (int)TimesheetStatuses.Created,
                HourLogEntries = distinctHourLogEntries,
                Attachments = attachments,
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

        [AbpAuthorize("Timesheet.Delete")]
        public async Task Delete(int id)
        {
            var timesheet = await Repository.GetAsync(id);
            if (timesheet.InvoiceId != null)
            {
                throw new UserFriendlyException("Cannot delete Timesheet. Invoice is already created");
            }

            var hourLogEntries = await HourLogEntryRepository.GetAll().Where(x => x.TimesheetId == id).ToListAsync();
            hourLogEntries.ForEach(x => x.TimesheetId = null);
            await Repository.DeleteAsync(id);
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


        private QueryBuilder<Timesheet, TimesheetQueryParameters> GetQuery(TimesheetQueryParameters queryParameter)
        {
            if (!queryParameter.StartTime.HasValue)
            {
                queryParameter.StartTime = DateTime.UtcNow.AddMonths(-1).StartOfMonth();
                queryParameter.EndTime = DateTime.UtcNow;
            }
            var query = QueryBuilder.Create<Timesheet, TimesheetQueryParameters>(Repository.GetAll());
            query.WhereIf(p => p.ProjectId.HasValue, p => x => x.ProjectId == p.ProjectId);
            query.WhereIf(p => p.ConsultantId.HasValue, p => x => x.Project.ConsultantId == p.ConsultantId);
            query.WhereIf(p => p.CompanyId.HasValue, p => x => x.Project.CompanyId == p.CompanyId);
            query.WhereIf(p => p.StatusId != null && p.StatusId.Length > 0, p => x => p.StatusId.Contains(x.StatusId));
            query.WhereIf(p => p.Name == "Invoiced" && p.StartTime.HasValue && p.EndTime.HasValue, p => x => x.StartDt >= p.StartTime && x.EndDt <= p.EndTime);

            var sorts = new Sorts<Timesheet>();
            sorts.Add(true, t => t.LastModificationTime,true,2);
            sorts.Add(true, t => t.CreationTime);
            query.ApplySorts(sorts);

            return query;

        }

        public async Task<Page<TimesheetListItemDto>> GetTimesheets(TimesheetQueryParameters queryParameter)
        {
            var query = GetQuery(queryParameter);
            var queryParameters = SavedQueries.Select(x => Mapper.Map(queryParameter, x)).ToList();
            var result = await query.ExecuteAsync<TimesheetListItemDto>(queryParameters.ToArray());
            return result;
        }


        public async Task<IEnumerable<MonthlySummary>> GetMonthlyHourReport(TimesheetQueryParameters queryParameter)
        {
            var query = GetQuery(queryParameter);
            return await query.ExecuteAsync((originalQuery) =>
                 (from t in originalQuery
                  select t.HourLogEntries).SelectMany(x => x).GetMonthlyHourReport()
            , queryParameter);
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