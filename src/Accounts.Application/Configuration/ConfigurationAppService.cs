using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Accounts.Configuration.Dto;

namespace Accounts.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : AccountsAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
