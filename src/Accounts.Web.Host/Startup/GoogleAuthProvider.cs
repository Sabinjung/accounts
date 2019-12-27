using Accounts.Authentication.External;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using System.IdentityModel.Tokens.Jwt;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading;

namespace Accounts.Web.Host.Startup
{
    public class GoogleAuthProvider : ExternalAuthProviderApiBase
    {
        public const string Name = "Google";


        public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
        {

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets() { ClientId = ProviderInfo.ClientId, ClientSecret = ProviderInfo.ClientSecret },
                Scopes = new[] { "profile" },
            });

            var credential = await flow.ExchangeCodeForTokenAsync("user-id", accessCode, ProviderInfo.CallbackUrl, CancellationToken.None);
            var idtokenpayload = await GoogleJsonWebSignature.ValidateAsync(credential.IdToken);


            return new ExternalAuthUserInfo()
            {
                EmailAddress = idtokenpayload.Email,
                Name = idtokenpayload.GivenName,
                Provider = ProviderInfo.Name,
                ProviderKey = idtokenpayload.Subject,
                Surname = idtokenpayload.FamilyName
            };

        }
    }
}
