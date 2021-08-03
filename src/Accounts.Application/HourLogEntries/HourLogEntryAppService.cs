using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Accounts.Data;
using Accounts.Data.Dto;
using Accounts.EntityFrameworkCore.Repositories;
using Accounts.Extensions;
using Accounts.HourLogEntries.Dto;
using Accounts.Models;
using Accounts.Core.Notify;
using Accounts.Projects.Dto;
using Accounts.Timesheets;
using Accounts.Timesheets.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoreLinq;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Abp.Extensions;

namespace Accounts.HourLogEntries
{
    [AbpAuthorize("Timesheet")]
    public class HourLogEntryAppService : AsyncCrudAppService<HourLogEntry, HourLogEntryDto>, IHourLogEntryAppService
    {
        private readonly IProjectRepository ProjectRepository;
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<Consultant> ConsultantRepository;

        private readonly IRepository<Timesheet> TimesheetRepository;

        private readonly IRepository<Config> ConfigRepository;
        
        private readonly IMapper Mapper;

        private readonly ITimesheetService TimesheetService;

        private readonly INotifyService NotifyService;

        private readonly QueryBuilderFactory QueryBuilderFactory;

        public HourLogEntryAppService(
            IRepository<HourLogEntry> repository,
            IProjectRepository projectRepository,
            IRepository<Project> projectRepo,
            IRepository<Consultant> consultantRepository,
            IRepository<Config> configRepository,
            ITimesheetService timesheetService,
            IRepository<Timesheet> timesheetRepository,
            QueryBuilderFactory queryBuilderFactory,
            INotifyService notifyService,
            IMapper mapper) : base(repository)
        {
            ConfigRepository = configRepository;
            ProjectRepository = projectRepository;
            _projectRepo = projectRepo;
            ConsultantRepository = consultantRepository;
            Mapper = mapper;
            TimesheetService = timesheetService;
            TimesheetRepository = timesheetRepository;
            NotifyService = notifyService;
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
                        existingHourLog.Hours = Math.Round((double)log.Hours,2);
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

        public async Task<Page<ProjectHourLogEntryDto>> GetProjectHourLogs
                (ProjectHourLogsQueryParameter hourLogQueryParameter)
        {
            var startDay = hourLogQueryParameter.StartDt;
            var endDay = hourLogQueryParameter.EndDt;

            var projects = ProjectRepository.PagedQueryActiveProjectsByDate(startDay, endDay, hourLogQueryParameter);
            var pagedResult = await projects.ExecuteAsync<ProjectDto>(hourLogQueryParameter);
            var activeProjectsQuery = pagedResult.Results.ToList();
            
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
               Task.FromResult(activeProjectsQuery),
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
                    TotalHrs = projectHourLog?.HourLogEntries.Where(x => x.Day >= uStartDt && x.Day <= uEndDt).Sum(x => x.Hours.HasValue ? Math.Round(x.Hours.Value,2) : 0)
                };
                proj.UpcomingTimesheetSummary = upcomingTimesheetSummary;

                proj.LastApprovedDate = projectLastApprovedTimesheet?.ApprovedDate;
                proj.LastInvoicedDate = projectLastInvoicedTimesheet?.InvoiceGeneratedDate;
                proj.TotalHoursBilled = Math.Round(proj.TotalHoursBilled, 2);
                proj.TotalAmountBilled = Math.Round(proj.TotalAmountBilled, 2);
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
            Page<ProjectHourLogEntryDto> finalResult = new Page<ProjectHourLogEntryDto>()
            {
                CurrentPage = pagedResult.CurrentPage,
                TotalCount = pagedResult.TotalCount,
                PageCount = pagedResult.PageCount,
                PageSize = pagedResult.PageSize,
                RecordCount = pagedResult.RecordCount,
                RecordCounts = pagedResult.RecordCounts
            };
            finalResult.Results = result.OrderBy(x => x.Project.ConsultantName);
            return finalResult;
        }
        [AbpAuthorize("HourLog.Report")]
        public async Task<List<HourLogReportDto>> GetHourLogReport()
        {
            var invoicedetails = await Repository.GetAll().Where(x => x.Timesheet.InvoiceId != null).ToListAsync();
            var projects = ProjectRepository.GetAll();
            var hourlogs = Repository.GetAll();
            var joinHourlog = projects.Join(hourlogs, x => x.Id, y => y.ProjectId, (project, hourlog) => new 
            { 
                hourlog.ProjectId,
                project.Consultant.DisplayName,
                hourlog.Day,
                hourlog.Hours,
                hourlog.Timesheet.InvoiceId,
                project.InvoiceCycleStartDt
            });
            var report =await joinHourlog.Where(x =>
                                 x.InvoiceId == null
                                 && x.Hours > 0
                                 && x.Day >= x.InvoiceCycleStartDt
                                 && x.Day < DateTime.Now.AddDays(-45))
                                .GroupBy(x=> new { x.ProjectId,x.DisplayName})
                                .Select(y=>new HourLogReportDto
                                {
                                    ProjectId = y.Key.ProjectId,
                                    DisplayName = y.Key.DisplayName,
                                    TotalInvoicedHours = Math.Round((double)invoicedetails.Where(z => z.ProjectId == y.Key.ProjectId).Sum(s => s.Hours),2),
                                    TotalNonInvoicedHours =Math.Round((double)y.Sum(s => s.Hours),2)
                                }).ToListAsync();
           
            return report;

        }
        public async Task<UnassociatedProjectHourlogReportDto> GetHourLogReportDetails(int projectId)
        {
            List<int> unassociatedTimesheets = TimesheetRepository.GetAllList().Where(x => x.InvoiceId == null).Select(x => x.Id).ToList();
            var unassociatedHoursProjects = await Repository.GetAllIncluding(x => x.Project)
                                            .Where(x => x.ProjectId == projectId 
                                            && x.Hours>0 && (x.TimesheetId == null || unassociatedTimesheets
                                            .Contains(x.TimesheetId.Value)) 
                                            && x.Day < DateTime.Now.AddDays(-45) 
                                            && x.Day >= x.Project.InvoiceCycleStartDt)
                                            .GroupBy(z => new
                                            {
                                                z.Day.Date.Month,
                                                z.Day.Date.Year
                                            }).ToListAsync();
            var result = new UnassociatedProjectHourlogReportDto
            {
                ConsultantName = ProjectRepository.FirstOrDefault(y => y.Id == projectId).Consultant.DisplayName,
                UnassociatedHourReportDtos = unassociatedHoursProjects.Select(x => new UnassociatedHourlogMonthReportDto
                {
                    MonthName = x.Key.Month,
                    Year = x.Key.Year,
                    Days = x.Where(y=>y.Hours>0).Select(y=>new DailyHour { Day =y.Day,Hour= y.Hours }),
                    TotalHours = (double)x.Sum(y => y.Hours)
                }).OrderBy(x => x.Year).ThenBy(x => x.MonthName)
            };
            return result;
        }
        public async Task<InvoicedHourLogEntryDto> GetInvoicedHourLogs(int invoiceId)
        {
            var timesheetId = TimesheetRepository.GetAll().Where(x => x.InvoiceId == invoiceId).FirstOrDefault().Id;
            var hourentries = await Repository.GetAll().Where(x => x.TimesheetId == timesheetId).Select(y => new HourLogDto
            {
                Hours = y.Hours,
                Day = y.Day,
                ProjectId = y.ProjectId
            })
            .OrderBy(x => x.Day)
            .ToListAsync();
            return new InvoicedHourLogEntryDto
            {
                HourLogEntries = hourentries
            };
        }

        [AbpAllowAnonymous]
        [HttpGet]
        public async Task<string> NotifyUnassociatedHours()
        {
            var result = await NotifyService.NotifyUser();
            return result;
        }
    }
}