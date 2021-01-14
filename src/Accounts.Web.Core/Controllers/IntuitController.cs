using Abp.Configuration;
using Accounts.Core.Invoicing;
using Accounts.Core.Invoicing.Intuit;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounts.Controllers
{
    public class IntuitController : AccountsControllerBase
    {
        private readonly OAuth2Client OAuth2Client;
        private readonly ISettingManager SettingManager;
        private readonly IAuthorizationService AuthorizationService;
        private readonly IntuitSettings IntuitSettings;
        public IntuitController(OAuth2Client oAuth2Client, ISettingManager settingManager, IAuthorizationService authorizationService, IOptions<IntuitSettings> options)
        {
            OAuth2Client = oAuth2Client;
            SettingManager = settingManager;
            AuthorizationService = authorizationService;
            IntuitSettings = options.Value;
        }


        [HttpGet]

        public async Task<IActionResult> Login(string returnUrl = "/")
        {

            List<OidcScopes> scopes = new List<OidcScopes>();
            scopes.Add(OidcScopes.Accounting);
            string authorizeUrl = OAuth2Client.GetAuthorizationURL(scopes, returnUrl);
            return Redirect(authorizeUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Callback(string state, string code, string realmId)
        {
            await GetAuthTokensAsync(code, realmId);
            return Redirect(state);
        }

        private async Task GetAuthTokensAsync(string code, string realmId)
        {
            var tokenResponse = await OAuth2Client.GetBearerTokenAsync(code);
            await SettingManager.ChangeSettingForApplicationAsync("Intuit.AccessToken", tokenResponse.AccessToken);
            await SettingManager.ChangeSettingForApplicationAsync("Intuit.RefreshToken", tokenResponse.RefreshToken);
            await SettingManager.ChangeSettingForApplicationAsync("Intuit.AccessTokenExpiresIn", DateTime.UtcNow.AddSeconds(tokenResponse.AccessTokenExpiresIn).ToString());
            await SettingManager.ChangeSettingForApplicationAsync("Intuit.RefreshTokenExpiresIn", DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn).ToString());
            await SettingManager.ChangeSettingForApplicationAsync("Intuit.RealmId", realmId);
        }
    }
}
