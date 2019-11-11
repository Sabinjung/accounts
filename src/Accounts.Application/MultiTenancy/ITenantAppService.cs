using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Accounts.MultiTenancy.Dto;

namespace Accounts.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

