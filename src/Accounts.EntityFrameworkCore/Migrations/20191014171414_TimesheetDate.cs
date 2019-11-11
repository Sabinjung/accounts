using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class TimesheetDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Timesheets",
                newName: "StartDt");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Timesheets",
                newName: "EndDt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDt",
                table: "Timesheets",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndDt",
                table: "Timesheets",
                newName: "EndDate");
        }
    }
}
