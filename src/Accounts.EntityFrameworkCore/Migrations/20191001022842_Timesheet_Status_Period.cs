using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class Timesheet_Status_Period : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetPeriod_PeriodId",
                table: "Timesheets");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetStatus_StatusId",
                table: "Timesheets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimesheetStatus",
                table: "TimesheetStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimesheetPeriod",
                table: "TimesheetPeriod");

            migrationBuilder.RenameTable(
                name: "TimesheetStatus",
                newName: "TimesheetStatuses");

            migrationBuilder.RenameTable(
                name: "TimesheetPeriod",
                newName: "TimesheetPeriods");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimesheetStatuses",
                table: "TimesheetStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimesheetPeriods",
                table: "TimesheetPeriods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetPeriods_PeriodId",
                table: "Timesheets",
                column: "PeriodId",
                principalTable: "TimesheetPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetStatuses_StatusId",
                table: "Timesheets",
                column: "StatusId",
                principalTable: "TimesheetStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetPeriods_PeriodId",
                table: "Timesheets");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetStatuses_StatusId",
                table: "Timesheets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimesheetStatuses",
                table: "TimesheetStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimesheetPeriods",
                table: "TimesheetPeriods");

            migrationBuilder.RenameTable(
                name: "TimesheetStatuses",
                newName: "TimesheetStatus");

            migrationBuilder.RenameTable(
                name: "TimesheetPeriods",
                newName: "TimesheetPeriod");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimesheetStatus",
                table: "TimesheetStatus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimesheetPeriod",
                table: "TimesheetPeriod",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetPeriod_PeriodId",
                table: "Timesheets",
                column: "PeriodId",
                principalTable: "TimesheetPeriod",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetStatus_StatusId",
                table: "Timesheets",
                column: "StatusId",
                principalTable: "TimesheetStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
