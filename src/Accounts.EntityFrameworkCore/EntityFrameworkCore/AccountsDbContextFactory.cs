using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Accounts.Configuration;
using Accounts.Web;

namespace Accounts.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AccountsDbContextFactory : IDesignTimeDbContextFactory<AccountsDbContext>
    {
        public AccountsDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AccountsDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AccountsDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AccountsConsts.ConnectionStringName));

            return new AccountsDbContext(builder.Options);
        }
    }
}
