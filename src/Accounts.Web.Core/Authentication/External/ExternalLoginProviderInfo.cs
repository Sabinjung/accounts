﻿using System;

namespace Accounts.Authentication.External
{
    public class ExternalLoginProviderInfo
    {
        public string Name { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string CallbackUrl { get; set; }

        public Type ProviderApiType { get; set; }

        public ExternalLoginProviderInfo(string name, string clientId, string clientSecret, string callbackUrl,  Type providerApiType)
        {
            Name = name;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ProviderApiType = providerApiType;
            CallbackUrl = callbackUrl;
        }
    }
}
