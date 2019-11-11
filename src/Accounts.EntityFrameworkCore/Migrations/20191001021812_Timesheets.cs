using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class Timesheets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimesheetPeriod",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    InvoiceCycleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetPeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimesheetStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimesheetStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_PeriodId",
                table: "Timesheets",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_StatusId",
                table: "Timesheets",
                column: "StatusId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetPeriod_PeriodId",
                table: "Timesheets");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetStatus_StatusId",
                table: "Timesheets");

            migrationBuilder.DropTable(
                name: "TimesheetPeriod");

            migrationBuilder.DropTable(
                name: "TimesheetStatus");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_PeriodId",
                table: "Timesheets");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_StatusId",
                table: "Timesheets");
        }
    }
}
