using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class IsTimeSheetCompletedentityaddedinProjectModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpensesTypes_ExpensesTypeId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Timesheets_TimeSheetId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpensesTypeId",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "TimeSheetId",
                table: "Expenses",
                newName: "TimesheetId");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_TimeSheetId",
                table: "Expenses",
                newName: "IX_Expenses_TimesheetId");

            migrationBuilder.AlterColumn<int>(
                name: "TimesheetId",
                table: "Expenses",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ExpenseTypeId",
                table: "Expenses",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpensesTypes_ExpenseTypeId",
                table: "Expenses",
                column: "ExpenseTypeId",
                principalTable: "ExpensesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Timesheets_TimesheetId",
                table: "Expenses",
                column: "TimesheetId",
                principalTable: "Timesheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpensesTypes_ExpenseTypeId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Timesheets_TimesheetId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseTypeId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpenseTypeId",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "TimesheetId",
                table: "Expenses",
                newName: "TimeSheetId");

            migrationBuilder.RenameIndex(
                name: "IX_Expenses_TimesheetId",
                table: "Expenses",
                newName: "IX_Expenses_TimeSheetId");

            migrationBuilder.AlterColumn<int>(
                name: "TimeSheetId",
                table: "Expenses",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpensesTypeId",
                table: "Expenses",
                column: "ExpensesTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpensesTypes_ExpensesTypeId",
                table: "Expenses",
                column: "ExpensesTypeId",
                principalTable: "ExpensesTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Timesheets_TimeSheetId",
                table: "Expenses",
                column: "TimeSheetId",
                principalTable: "Timesheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
