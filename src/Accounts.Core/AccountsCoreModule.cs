using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using Accounts.Authorization.Roles;
using Accounts.Authorization.Users;
using Accounts.Configuration;
using Accounts.Invoicing;
using Accounts.Localization;
using Accounts.Models;
using Accounts.MultiTenancy;
using Accounts.Timing;
using AutoMapper;
using PQ;

namespace Accounts
{
    [DependsOn(typeof(PQModule))]
    [DependsOn(typeof(AbpZeroCoreModule))]
    public class AccountsCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            AccountsLocalizationConfigurer.Configure(Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = AccountsConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AccountsCoreModule).GetAssembly());
            Configuration.Modules.AbpAutoMapper().Configurators.Add(
               // Scan the assembly for classes which inherit from AutoMapper.Profile
               cfg =>
               {
                   cfg.CreateMap<Timesheet, Invoice>()
                      .ConvertUsing<TimesheetToInvoiceConverter>();
               }

           );
        }

        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;

        }
    }
}
