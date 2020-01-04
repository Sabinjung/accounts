using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class FixingTheErrorOnExpensesAndLineItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "LineItem",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "ReportDt",
                table: "LineItem",
                newName: "ServiceDt");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "LineItem",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ReportDt",
                table: "Expenses",
                newName: "ServiceDt");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Expenses",
                newName: "Description");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Expenses",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "ServiceDt",
                table: "LineItem",
                newName: "ReportDt");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "LineItem",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "LineItem",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "ServiceDt",
                table: "Expenses",
                newName: "ReportDt");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Expenses",
                newName: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "Expenses",
                nullable: false,
                defaultValue: 0);
        }
    }
}
