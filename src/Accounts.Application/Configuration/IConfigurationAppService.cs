using System.Threading.Tasks;
using Accounts.Configuration.Dto;

namespace Accounts.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
