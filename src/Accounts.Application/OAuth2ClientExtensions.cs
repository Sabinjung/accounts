using Abp.Authorization;
using Abp.Configuration;
using Intuit.Ipp.OAuth2PlatformClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts
{
    public static class OAuth2ClientExtensions
    {

        public static async Task<bool> EstablishConnection(this OAuth2Client oAuth2Client, ISettingManager settingManager)
        {
            var intuitAccessToken = settingManager.GetSettingValueForApplication("Intuit.AccessToken");
            var intuitAccessTokenExpiresIn = settingManager.GetSettingValueForApplication("Intuit.AccessTokenExpiresIn");
            var intuitRefreshtoken = settingManager.GetSettingValueForApplication("Intuit.RefreshToken");
            var intuitRefreshTokenExpiresIn = settingManager.GetSettingValueForApplication("Intuit.RefreshTokenExpiresIn");
            var expirationExpiresIn = DateTime.Parse(intuitAccessTokenExpiresIn);
            if (string.IsNullOrEmpty(intuitAccessToken)) throw new AbpAuthorizationException();
            if (DateTime.Parse(intuitRefreshTokenExpiresIn) < DateTime.UtcNow) throw new AbpAuthorizationException();

            if (DateTime.UtcNow > expirationExpiresIn)
            {
                var tokenResponse = await oAuth2Client.RefreshTokenAsync(intuitRefreshtoken);
                await settingManager.ChangeSettingForApplicationAsync("Intuit.AccessToken", tokenResponse.AccessToken);
                await settingManager.ChangeSettingForApplicationAsync("Intuit.RefreshToken", tokenResponse.RefreshToken);
                await settingManager.ChangeSettingForApplicationAsync("Intuit.AccessTokenExpiresIn", DateTime.UtcNow.AddSeconds(tokenResponse.AccessTokenExpiresIn).ToString());
                await settingManager.ChangeSettingForApplicationAsync("Intuit.RefreshTokenExpiresIn", DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn).ToString());
            }

            return true;
        }
    }
}
