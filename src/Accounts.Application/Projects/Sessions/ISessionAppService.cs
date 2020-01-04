using System.Threading.Tasks;
using Abp.Application.Services;
using Accounts.Sessions.Dto;

namespace Accounts.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
