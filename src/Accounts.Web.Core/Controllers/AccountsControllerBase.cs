using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Accounts.Controllers
{
    public abstract class AccountsControllerBase: AbpController
    {
        protected AccountsControllerBase()
        {
            LocalizationSourceName = AccountsConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
