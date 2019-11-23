using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class TimesheetApproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "InvoiceGeneratedByUserId",
                table: "Timesheets",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ApprovedByUserId",
                table: "Timesheets",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_ApprovedByUserId",
                table: "Timesheets",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_CreatorUserId",
                table: "Timesheets",
                column: "CreatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_AbpUsers_ApprovedByUserId",
                table: "Timesheets",
                column: "ApprovedByUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_AbpUsers_CreatorUserId",
                table: "Timesheets",
                column: "CreatorUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_AbpUsers_ApprovedByUserId",
                table: "Timesheets");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_AbpUsers_CreatorUserId",
                table: "Timesheets");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_ApprovedByUserId",
                table: "Timesheets");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_CreatorUserId",
                table: "Timesheets");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceGeneratedByUserId",
                table: "Timesheets",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ApprovedByUserId",
                table: "Timesheets",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
