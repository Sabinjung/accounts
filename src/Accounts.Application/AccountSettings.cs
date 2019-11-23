using Abp.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts
{
    public class AccountsSettingsProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
                {
                    new SettingDefinition(
                        "Intuit.AccessToken",
                        null,
                        scopes:SettingScopes.Application
                        ),

                     new SettingDefinition(
                        "Intuit.AccessTokenExpiresIn",
                        null,
                         scopes:SettingScopes.Application
                        ),

                    new SettingDefinition(
                        "Intuit.RefreshToken",
                        null,
                         scopes:SettingScopes.Application
                        ),
                     new SettingDefinition(
                        "Intuit.RefreshTokenExpiresIn",
                        null,
                         scopes:SettingScopes.Application
                        ),

                      new SettingDefinition(
                        "Intuit.RealmId",
                        null,
                         scopes:SettingScopes.Application
                        ),


                };
        }
    }
}
