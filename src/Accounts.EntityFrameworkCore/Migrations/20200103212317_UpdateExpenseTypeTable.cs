using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class UpdateExpenseTypeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpensesTypes_ExpenseTypeId",
                table: "Expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpensesTypes",
                table: "ExpensesTypes");

            migrationBuilder.RenameTable(
                name: "ExpensesTypes",
                newName: "ExpenseTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseTypes",
                table: "ExpenseTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId",
                principalTable: "ExpenseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                table: "Expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseTypes",
                table: "ExpenseTypes");

            migrationBuilder.RenameTable(
                name: "ExpenseTypes",
                newName: "ExpensesTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpensesTypes",
                table: "ExpensesTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpensesTypes_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId",
                principalTable: "ExpensesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
