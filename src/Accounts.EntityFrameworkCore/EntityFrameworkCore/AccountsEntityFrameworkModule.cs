using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using Accounts.EntityFrameworkCore.Seed;
using Accounts.Models;

namespace Accounts.EntityFrameworkCore
{
    [DependsOn(
        typeof(AccountsCoreModule),
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class AccountsEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<AccountsDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        AccountsDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        AccountsDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                 });

                Configuration.EntityHistory.Selectors.Add(new Abp.NamedTypeSelector("Accounts.Models", type =>
                type.IsAssignableFrom(typeof(Attachment)) ||
                type.IsAssignableFrom(typeof(Project)) ||
                type.IsAssignableFrom(typeof(Invoice)) ||
                type.IsAssignableFrom(typeof(Consultant))||
                type.IsAssignableFrom(typeof(Timesheet)) ||
                type.IsAssignableFrom(typeof(HourLogEntry))));
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AccountsEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
