using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.EntityFrameworkCore.Repositories;
using Accounts.HourLogEntries.Dto;
using Accounts.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounts.Extensions;
using System.Collections.Concurrent;
using Accounts.Projects.Dto;
using AutoMapper.QueryableExtensions;
using Abp.Collections.Extensions;
using AutoMapper;
using Abp.Authorization;
using Accounts.Timesheets;
using Accounts.Data;

namespace Accounts.HourLogEntries
{
    [AbpAuthorize("Timesheet")]
    public class HourLogEntryAppService : AsyncCrudAppService<HourLogEntry, HourLogEntryDto>, IHourLogEntryAppService
    {
        private readonly IProjectRepository ProjectRepository;

        private readonly IMapper Mapper;

        private readonly ITimesheetService TimesheetService;

        public HourLogEntryAppService(
            IRepository<HourLogEntry> repository,
            IProjectRepository projectRepository,
            ITimesheetService timesheetService,
            IMapper mapper) : base(repository)
        {
            ProjectRepository = projectRepository;
            Mapper = mapper;
            TimesheetService = timesheetService;
        }

        [AbpAuthorize("Timesheet.LogHour")]
        public async Task AddUpdateHourLogs(IEnumerable<HourLogEntryDto> projectsHourLogs)
        {
            var addedHourLogs = new ConcurrentBag<HourLogEntry>();

            var hourLogEntries = await Repository.GetAllListAsync(x =>
            projectsHourLogs.Any(y => y.ProjectId == x.ProjectId && x.Day == y.Day));

            Parallel.ForEach(projectsHourLogs, log =>
            {
                var existingHourLog = hourLogEntries.FirstOrDefault(x => x.ProjectId == log.ProjectId && x.Day == log.Day);
                if (existingHourLog != null)
                {
                    existingHourLog.Hours = log.Hours;
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


        public async Task<IEnumerable<ProjectHourLogEntryDto>> GetProjectHourLogs
            (DateTime startDt, DateTime endDt, int? projectId, int? consultantId)
        {
            var startDay = startDt.Date;
            var endDay = endDt.Date;

            var activeProjectsQuery = ProjectRepository.QueryActiveProjects(startDay, endDay)
                .Where(projectId.HasValue, x => x.Id == projectId)
                .Where(consultantId.HasValue, x => x.ConsultantId == consultantId);


            var lastTimesheetQuery = from p in activeProjectsQuery
                                     let lT = p.Timesheets.OrderByDescending(x => x.EndDt).FirstOrDefault()
                                     select lT;



            var query = from log in Repository.GetAll()
                        where log.Day >= startDay && log.Day <= endDay && activeProjectsQuery.Any(x => x.Id == log.ProjectId)
                        group log by log.ProjectId into projectLogs
                        select new
                        {
                            ProjectId = projectLogs.Key,
                            HourLogEntries = projectLogs.Select(plog => new HourLogEntryDto
                            {
                                Day = plog.Day,
                                Hours = plog.Hours,
                                ProjectId = plog.ProjectId,
                                IsAssociatedWithTimesheet = plog.TimesheetId.HasValue ? true : false
                            })
                        };

            var (activeProjects, projectsHourLogs) = await TaskEx.WhenAll(
                Mapper.ProjectTo<ProjectDto>(activeProjectsQuery).ToListAsync(),
                query.ToListAsync());

            var lastTimesheets = await lastTimesheetQuery.ToListAsync();


            var result = activeProjects.Select(proj =>
            {
                var projectHourLog = projectsHourLogs.FirstOrDefault(y => y.ProjectId == proj.Id);
                var projectLastTimesheet = lastTimesheets.FirstOrDefault(t => t != null && t.ProjectId == proj.Id);
                var (uStartDt, uEndDt) = TimesheetService.CalculateTimesheetPeriod(proj.StartDt, proj.EndDt, (InvoiceCycles)proj.InvoiceCycleId, projectLastTimesheet?.EndDt);
                var duedays = projectLastTimesheet != null ? Math.Ceiling((DateTime.UtcNow - uStartDt).TotalDays) : Math.Ceiling((DateTime.UtcNow - uEndDt).TotalDays);
                proj.PastTimesheetDays = duedays > 0 ? duedays : 0;
                return new ProjectHourLogEntryDto
                {
                    Project = proj,
                    HourLogEntries = projectHourLog?.HourLogEntries ?? new List<HourLogEntryDto>()
                };
            });

            return result;
        }
    }
}
