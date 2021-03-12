using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Accounts.Configuration;
using Accounts.Authentication.External;
using Accounts.AzureServices;

namespace Accounts.Web.Host.Startup
{
    [DependsOn(
       typeof(AccountsWebCoreModule))]
    public class AccountsWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AccountsWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AccountsWebHostModule).GetAssembly());


        }

        public override void PostInitialize()
        {
            var externalAuthConfiguration = IocManager.Resolve<IExternalAuthConfiguration>();
            externalAuthConfiguration.Providers.Add(
                 new ExternalLoginProviderInfo(
                    GoogleAuthProvider.Name,
                    _appConfiguration["Authentication:Google:ClientId"],
                    _appConfiguration["Authentication:Google:ClientSecret"],
                    _appConfiguration["Authentication:Google:CallbackUrl"],
                    typeof(GoogleAuthProvider)
                )
            );

            var rmanager = IocManager.Resolve<IAzureServiceBus>();
            rmanager.ServiceBusListener();
        }
    }
}
