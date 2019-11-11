using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class TimesheetPeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetPeriods_PeriodId",
                table: "Timesheets");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_PeriodId",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "PeriodId",
                table: "Timesheets");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Timesheets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Timesheets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Timesheets");

            migrationBuilder.AddColumn<int>(
                name: "PeriodId",
                table: "Timesheets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_PeriodId",
                table: "Timesheets",
                column: "PeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetPeriods_PeriodId",
                table: "Timesheets",
                column: "PeriodId",
                principalTable: "TimesheetPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
