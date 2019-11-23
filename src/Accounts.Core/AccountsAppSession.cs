using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts.Core
{
    public class AccountsAppSession : ClaimsAbpSession, ITransientDependency
    {

        private readonly ISettingManager SettingManager;
        public AccountsAppSession(
        IPrincipalAccessor principalAccessor,
        IMultiTenancyConfig multiTenancy,
        ITenantResolver tenantResolver,
        IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider,
        ISettingManager settingManager) :
        base(principalAccessor, multiTenancy, tenantResolver, sessionOverrideScopeProvider)
        {
            SettingManager = settingManager;
        }

        public string RealmId
        {
            get
            {
                return SettingManager.GetSettingValueForApplication("Intuit.RealmId");

            }
        }
        public string AccessToken
        {
            get
            {
                return SettingManager.GetSettingValueForApplication("Intuit.AccessToken");
            }
        }
    }
}
