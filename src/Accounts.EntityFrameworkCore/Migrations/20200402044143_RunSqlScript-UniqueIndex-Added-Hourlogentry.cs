using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class RunSqlScriptUniqueIndexAddedHourlogentry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX IX_HourLogEntries_ProjectId_Day_IsDeleted ON dbo.HourLogEntries(ProjectId, Day, IsDeleted); ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
