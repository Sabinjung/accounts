using Abp.Application.Services;
using Accounts.EntityFrameworkCore.Repositories;
using Accounts.HourLogEntries.Dto;
using Accounts.Models;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounts.HourLogEntries
{
    public interface IHourLogEntryAppService : IAsyncCrudAppService<HourLogEntryDto>
    {
        Task<Page<ProjectHourLogEntryDto>> GetProjectHourLogs(
            ProjectHourLogsQueryParameter projectHourLogsQueryParameter);

        Task AddUpdateHourLogs(IEnumerable<HourLogEntryDto> projectsHourLogs);
    }
}