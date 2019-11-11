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

namespace Accounts.HourLogEntries
{
    public class HourLogEntryAppService : AsyncCrudAppService<HourLogEntry, HourLogEntryDto>, IHourLogEntryAppService
    {
        private readonly IProjectRepository ProjectRepository;

        private readonly IMapper Mapper;

        public HourLogEntryAppService(
            IRepository<HourLogEntry> repository,
            IProjectRepository projectRepository,
            IMapper mapper) : base(repository)
        {
            ProjectRepository = projectRepository;
            Mapper = mapper;
        }

        //[AbpAuthorize("Timesheet.LogHours")]
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
                                ProjectId = plog.ProjectId

                            })
                        };

            var (activeProjects, projectsHourLogs) = await TaskEx.WhenAll(
                Mapper.ProjectTo<ProjectDto>(activeProjectsQuery).ToListAsync(),
                query.ToListAsync());


            return activeProjects.AsParallel().Select(proj =>
            {
                var projectHourLog = projectsHourLogs.First(y => y.ProjectId == proj.Id);
                return new ProjectHourLogEntryDto
                {
                    Project = proj,
                    HourLogEntries = projectHourLog.HourLogEntries
                };

            });

        }
    }
}
