using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Accounts.Data;
using Accounts.Data.Dto;
using Accounts.EntityFrameworkCore.Repositories;
using Accounts.Extensions;
using Accounts.HourLogEntries.Dto;
using Accounts.Models;
using Accounts.Projects.Dto;
using Accounts.Timesheets;
using Accounts.Timesheets.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Accounts.HourLogEntries
{
    [AbpAuthorize("Timesheet")]
    public class HourLogEntryAppService : AsyncCrudAppService<HourLogEntry, HourLogEntryDto>, IHourLogEntryAppService
    {
        private readonly IProjectRepository ProjectRepository;

        private readonly IRepository<Timesheet> TimesheetRepository;

        private readonly IMapper Mapper;

        private readonly ITimesheetService TimesheetService;

        private readonly QueryBuilderFactory QueryBuilderFactory;

        public HourLogEntryAppService(
            IRepository<HourLogEntry> repository,
            IProjectRepository projectRepository,
            ITimesheetService timesheetService,
            IRepository<Timesheet> timesheetRepository,
            QueryBuilderFactory queryBuilderFactory,
            IMapper mapper) : base(repository)
        {
            ProjectRepository = projectRepository;
            Mapper = mapper;
            TimesheetService = timesheetService;
            TimesheetRepository = timesheetRepository;
            QueryBuilderFactory = queryBuilderFactory;
        }

        [AbpAuthorize("Timesheet.LogHour")]
        public async Task AddUpdateHourLogs(IEnumerable<HourLogEntryDto> projectsHourLogs)
        {
            var distinctHourLogs = projectsHourLogs.DistinctBy(x => new { x.Day, x.ProjectId });
            var addedHourLogs = new ConcurrentBag<HourLogEntry>();

            var hourLogEntries = await Repository.GetAllListAsync(x =>
            distinctHourLogs.Any(y => y.ProjectId == x.ProjectId && x.Day == y.Day));

            Parallel.ForEach(distinctHourLogs, log =>
            {
                var existingHourLog = hourLogEntries.FirstOrDefault(x => x.ProjectId == log.ProjectId && x.Day == log.Day);
                if (existingHourLog != null)
                {
                    if (log.Hours.HasValue)
                    {
                        existingHourLog.Hours = log.Hours;
                    }
                }
                else
                {
                    addedHourLogs.Add(Mapper.Map<HourLogEntry>(log));
                }
            });

            foreach (var newLog in addedHourLogs)
            {
                Repository.Insert(newLog);
            }
        }

        public async Task<IEnumerable<HourMonthlyReport>> GetProjectMonthlyHourReport(HourLogQueryParameter queryParameter)
        {
            var query = QueryBuilderFactory.Create<HourLogEntry, HourLogQueryParameter>(Repository.GetAll());
            query.WhereIf(h => h.ProjectId.HasValue, h => x => x.ProjectId == h.ProjectId);
            query.WhereIf(h => h.StartDt.HasValue, h => x => x.Day.Date >= h.StartDt.Value.Date);
            query.WhereIf(h => h.EndDt.HasValue, h => x => x.Day.Date <= h.EndDt.Value.Date);
            query.WhereIf(h => true, h => x => x.Timesheet != null && (x.Timesheet.ApprovedDate.HasValue));

            return await query.ExecuteAsync((originalQuery) =>
                    from proj in ProjectRepository.GetAll()
                    join p in (from hl in originalQuery
                               group hl by new { hl.ProjectId } into g
                               select new HourMonthlyReport
                               {
                                   ProjectId = g.Key.ProjectId,
                                   MonthlySummaries = from mhl in g
                                                      group mhl by new { mhl.Day.Month, mhl.Day.Year, } into mg
                                                      select new MonthlySummary
                                                      {
                                                          ProjectId = g.Key.ProjectId,
                                                          Month = mg.Key.Month,
                                                          Year = mg.Key.Year,
                                                          Value = mg.Sum(y => y.Hours ?? 0),
                                                      }
                               }) on proj.Id equals p.ProjectId into s
                    from ms in s.DefaultIfEmpty()
                    let consultantName = proj.Consultant.FirstName + " " + proj.Consultant.LastName
                    let companyName = proj.Company.DisplayName
                    orderby proj.Consultant.FirstName
                    select new HourMonthlyReport
                    {
                        ProjectId = proj.Id,
                        ConsultantName = consultantName,
                        CompanyName = companyName,
                        IsProjectActive = proj.EndDt.HasValue ? proj.EndDt > DateTime.UtcNow : true,
                        MonthlySummaries = ms.MonthlySummaries
                    }
            , queryParameter);
        }

        public async Task<List<ProjectHourLogReport>> GetPayrollHourLogsReport(ProjectParameter queryParameter)
        {
            var startDay = new DateTime(queryParameter.Year, queryParameter.Month, 1);
            var endDay = startDay.AddMonths(1);

            List<DailyHourLogDetails> hourLogs = await Repository.GetAll().Where(x => x.Day >= startDay && x.Day < endDay)
                .Select(y => new DailyHourLogDetails()
                {
                    Day = y.Day,
                    ProjectId = y.ProjectId,
                    CompanyName = y.Project.Company.DisplayName,
                    ConsultantName = y.Project.Consultant.DisplayName,
                    Hours = y.Hours,
                    Status = y.Timesheet.Status.Name
                }).ToListAsync();

            var activeProjects = ProjectRepository.QueryActiveProjectsByDate(startDay, endDay);
            var activeProjectsHourLogReports = (from ac in activeProjects
                                                join hl in hourLogs on ac.Id equals hl.ProjectId into ah
                                                from p in ah.DefaultIfEmpty()
                                                group new { ac, p } by new { ac.Id, ConsultantName = ac.Consultant.DisplayName, CompanyName = ac.Company.DisplayName, IsActive = ac.EndDt >= DateTime.Now || ac.EndDt == null }
                                                into ach
                                                select new ProjectHourLogReport
                                                {
                                                    ProjectId = ach.Key.Id,
                                                    CompanyName = ach.Key.CompanyName,
                                                    ConsultantName = ach.Key.ConsultantName,
                                                    IsActive = ach.Key.IsActive,
                                                }).ToList();

            var lastApprovedTimeSheetQuery = from t in TimesheetRepository.GetAll()
                                             where activeProjects.Any(x => x.Id == t.ProjectId) && t.ApprovedDate.HasValue
                                             group t by t.ProjectId into g
                                             select g.OrderByDescending(x => x.EndDt).First();
            var lastApproved = await lastApprovedTimeSheetQuery.ToListAsync();

            foreach (var projectHourLogReport in activeProjectsHourLogReports)
            {
                var dailyhourlogs = hourLogs.Where(x => x.ProjectId == projectHourLogReport.ProjectId)
                    .Select(y => new DailyHourLog()
                    {
                        Day = y.Day,
                        Hours = y.Hours,
                        Status = y.Status
                    });
                projectHourLogReport.LastApprovedDate = lastApproved.FirstOrDefault(t => t != null && t.ProjectId == projectHourLogReport.ProjectId) != null ? lastApproved.FirstOrDefault(t => t != null && t.ProjectId == projectHourLogReport.ProjectId).ApprovedDate : (DateTime?)null;
                projectHourLogReport.DailyHourLogs.AddRange(dailyhourlogs);
            }

            return activeProjectsHourLogReports;
        }

        public async Task<IEnumerable<ProjectHourLogEntryDto>> GetProjectHourLogs
                (DateTime startDt, DateTime endDt, int? projectId, int? consultantId)
        {
            var startDay = startDt.Date;
            var endDay = endDt.Date;

            var activeProjectsQuery = ProjectRepository.QueryActiveProjectsByDate(startDay, endDay)
                .Where(projectId.HasValue, x => x.Id == projectId)
                .Where(consultantId.HasValue, x => x.ConsultantId == consultantId);

            var lastTimesheetQuery = from t in TimesheetRepository.GetAll()
                                     where activeProjectsQuery.Any(x => x.Id == t.ProjectId)
                                     group t by t.ProjectId into g
                                     select g.OrderByDescending(x => x.EndDt).First();

            var lastApprovedTimeSheetQuery = from t in TimesheetRepository.GetAll()
                                             where activeProjectsQuery.Any(x => x.Id == t.ProjectId) && t.ApprovedDate.HasValue
                                             group t by t.ProjectId into g
                                             select g.OrderByDescending(x => x.EndDt).First();
            var lastInvoicedTimesheetQuery = from t in TimesheetRepository.GetAll()
                                             where activeProjectsQuery.Any(x => x.Id == t.ProjectId) && t.InvoiceGeneratedDate.HasValue
                                             group t by t.ProjectId into g
                                             select g.OrderByDescending(x => x.EndDt).First();

            var query = from log in Repository.GetAll()
                        where log.Day >= startDay && log.Day <= endDay && activeProjectsQuery.Any(x => x.Id == log.ProjectId)
                        group log by log.ProjectId into projectLogs
                        select new
                        {
                            ProjectId = projectLogs.Key,
                            HourLogEntries = projectLogs.Select(plog => new HourLogEntryDto
                            {
                                Day = plog.Day,
                                Hours = plog.Hours.HasValue ? plog.Hours.Value : 0,
                                ProjectId = plog.ProjectId,
                                IsAssociatedWithTimesheet = plog.TimesheetId.HasValue && plog.Timesheet.StatusId != (int)TimesheetStatuses.Created ? true : false,
                                TimesheetStatusesId = plog.TimesheetId.HasValue ? plog.Timesheet.StatusId : (int)TimesheetStatuses.TimeSheetOpen
                            })
                        };

            var (activeProjects, projectsHourLogs) = await TaskEx.WhenAll(
                Mapper.ProjectTo<ProjectDto>(activeProjectsQuery).ToListAsync(),
                query.ToListAsync());

            var (lastApproved, lastInvoiced) = await TaskEx.WhenAll(lastApprovedTimeSheetQuery.ToListAsync(), lastInvoicedTimesheetQuery.ToListAsync());
            var lastTimesheets = await lastTimesheetQuery.ToListAsync();

            var result = activeProjects.AsParallel().Select(proj =>
            {
                var projectHourLog = projectsHourLogs.FirstOrDefault(y => y.ProjectId == proj.Id);
                var projectLastTimesheet = lastTimesheets.FirstOrDefault(t => t != null && t.ProjectId == proj.Id);
                var projectLastApprovedTimesheet = lastApproved.FirstOrDefault(t => t != null && t.ProjectId == proj.Id);
                var projectLastInvoicedTimesheet = lastInvoiced.FirstOrDefault(t => t != null && t.ProjectId == proj.Id);
                var (uStartDt, uEndDt) = TimesheetService.CalculateTimesheetPeriod(proj.StartDt, proj.EndDt, proj.InvoiceCycleStartDt, (InvoiceCycles)proj.InvoiceCycleId, projectLastTimesheet?.EndDt);
                var duedays = projectLastTimesheet != null ? Math.Ceiling((DateTime.UtcNow - uStartDt).TotalDays) : Math.Ceiling((DateTime.UtcNow - uEndDt).TotalDays);

                var upcomingTimesheetSummary = new TimesheetSummary
                {
                    StartDt = uStartDt,
                    EndDt = uEndDt,
                    TotalHrs = projectHourLog?.HourLogEntries.Where(x => x.Day >= uStartDt && x.Day <= uEndDt).Sum(x => x.Hours.HasValue ? x.Hours.Value : 0)
                };
                proj.UpcomingTimesheetSummary = upcomingTimesheetSummary;

                proj.LastApprovedDate = projectLastApprovedTimesheet?.ApprovedDate;
                proj.LastInvoicedDate = projectLastInvoicedTimesheet?.InvoiceGeneratedDate;

                proj.PastTimesheetDays = duedays > 0 ? duedays : 0;
                if (duedays > 0)
                {
                    proj.TimesheetStatus = TimesheetStatuses.TimeSheetOpen;
                }
                else
                {
                    proj.TimesheetStatus = projectLastTimesheet != null
                    ? (TimesheetStatuses)projectLastTimesheet.StatusId
                    : TimesheetStatuses.TimeSheetOpen;
                }
                return new ProjectHourLogEntryDto
                {
                    Project = proj,
                    HourLogEntries = projectHourLog?.HourLogEntries ?? new List<HourLogEntryDto>()
                };
            });

            return result.OrderBy(x => x.Project.ConsultantName);
        }
    }
}