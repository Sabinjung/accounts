using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class UpdatindExpensesAndExpensesType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpensesName",
                table: "ExpensesTypes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Expenses",
                newName: "Comment");

            migrationBuilder.DropColumn(
                name: "ExpensesTypeId",
                table: "LineItem");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ExpensesTypes",
                newName: "ExpensesName");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Expenses",
                newName: "Description");

            migrationBuilder.DropColumn(
                name: "ExpensesTypeId",
                table: "LineItem");
        }
    }
}
