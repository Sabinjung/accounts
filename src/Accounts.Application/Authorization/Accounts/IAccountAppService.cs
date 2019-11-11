using System.Threading.Tasks;
using Abp.Application.Services;
using Accounts.Authorization.Accounts.Dto;

namespace Accounts.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
