using Abp.Application.Services;
using Accounts.HourLogEntries.Dto;
using Accounts.Timesheets.Dto;
using System.Threading.Tasks;

namespace Accounts.Projects
{
    public interface ITimesheetAppService
    {
        Task Approve(int timesheetId);
        Task Create(CreateTimesheetInputDto input);

    }
}