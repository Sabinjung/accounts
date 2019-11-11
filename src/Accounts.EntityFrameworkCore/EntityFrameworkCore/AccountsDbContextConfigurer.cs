using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Accounts.EntityFrameworkCore
{
    public static class AccountsDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AccountsDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AccountsDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
