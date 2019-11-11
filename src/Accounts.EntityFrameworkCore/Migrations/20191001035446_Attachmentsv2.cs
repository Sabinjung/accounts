using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class Attachmentsv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetStatuses_StatusId",
                table: "Timesheets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimesheetStatuses",
                table: "TimesheetStatuses");

            migrationBuilder.RenameTable(
                name: "TimesheetStatuses",
                newName: "TimesheetStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimesheetStatus",
                table: "TimesheetStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetStatus_StatusId",
                table: "Timesheets",
                column: "StatusId",
                principalTable: "TimesheetStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetStatus_StatusId",
                table: "Timesheets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimesheetStatus",
                table: "TimesheetStatus");

            migrationBuilder.RenameTable(
                name: "TimesheetStatus",
                newName: "TimesheetStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimesheetStatuses",
                table: "TimesheetStatuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetStatuses_StatusId",
                table: "Timesheets",
                column: "StatusId",
                principalTable: "TimesheetStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
