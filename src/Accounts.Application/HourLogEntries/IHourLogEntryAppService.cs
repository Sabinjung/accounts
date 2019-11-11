using Abp.Application.Services;
using Accounts.HourLogEntries.Dto;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounts.HourLogEntries
{
    public interface IHourLogEntryAppService : IAsyncCrudAppService<HourLogEntryDto>
    {
        Task<IEnumerable<ProjectHourLogEntryDto>> GetProjectHourLogs(
            DateTime startDt, DateTime endDt, int? projectId, int? consultantId);

        Task AddUpdateHourLogs(IEnumerable<HourLogEntryDto> projectsHourLogs);
    }
}