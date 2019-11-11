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
        public AccountsAppSession(
        IPrincipalAccessor principalAccessor,
        IMultiTenancyConfig multiTenancy,
        ITenantResolver tenantResolver,
        IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider) :
        base(principalAccessor, multiTenancy, tenantResolver, sessionOverrideScopeProvider)
        {

        }

        public string RealmId
        {
            get
            {
                var realmId = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "realmid");
                if (string.IsNullOrEmpty(realmId?.Value))
                {
                    return null;
                }

                return realmId.Value;
            }
        }
        public string AccessToken
        {
            get
            {
                var accessToken = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == "access_token");
                if (string.IsNullOrEmpty(accessToken?.Value))
                {
                    return null;
                }

                return accessToken.Value;
            }
        }
    }
}
