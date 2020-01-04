using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class ExpenseTypeReportDt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "ServiceDt",
                table: "Expenses",
                newName: "ReportDt");

            migrationBuilder.AlterColumn<int>(
                name: "ExpenseTypeId",
                table: "Expenses",
                nullable: true,
                oldClrType: typeof(int));

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

            migrationBuilder.RenameColumn(
                name: "ReportDt",
                table: "Expenses",
                newName: "ServiceDt");

            migrationBuilder.AlterColumn<int>(
                name: "ExpenseTypeId",
                table: "Expenses",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseTypes_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId",
                principalTable: "ExpenseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
