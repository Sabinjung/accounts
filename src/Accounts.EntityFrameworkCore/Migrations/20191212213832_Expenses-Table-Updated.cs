using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class ExpensesTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReportDt",
                table: "Expenses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReportDt",
                table: "Expenses",
                nullable: true,
                oldClrType: typeof(DateTime));
        }
    }
}
