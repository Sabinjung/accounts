using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class Timesheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimesheetId",
                table: "HourLogEntries",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Timesheets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    PeriodId = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    ApprovedByUserId = table.Column<int>(nullable: true),
                    ApprovedDate = table.Column<DateTime>(nullable: true),
                    InvoiceDescriptorId = table.Column<int>(nullable: true),
                    InvoiceGeneratedByUserId = table.Column<int>(nullable: true),
                    InvoiceGeneratedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timesheets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HourLogEntries_TimesheetId",
                table: "HourLogEntries",
                column: "TimesheetId");

            migrationBuilder.AddForeignKey(
                name: "FK_HourLogEntries_Timesheets_TimesheetId",
                table: "HourLogEntries",
                column: "TimesheetId",
                principalTable: "Timesheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HourLogEntries_Timesheets_TimesheetId",
                table: "HourLogEntries");

            migrationBuilder.DropTable(
                name: "Timesheets");

            migrationBuilder.DropIndex(
                name: "IX_HourLogEntries_TimesheetId",
                table: "HourLogEntries");

            migrationBuilder.DropColumn(
                name: "TimesheetId",
                table: "HourLogEntries");
        }
    }
}
