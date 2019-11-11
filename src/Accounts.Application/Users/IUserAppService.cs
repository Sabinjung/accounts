using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Accounts.Roles.Dto;
using Accounts.Users.Dto;

namespace Accounts.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
