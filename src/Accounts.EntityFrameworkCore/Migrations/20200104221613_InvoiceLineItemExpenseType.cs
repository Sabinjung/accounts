using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class InvoiceLineItemExpenseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpenseTypeId",
                table: "LineItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_ExpenseTypeId",
                table: "LineItems",
                column: "ExpenseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_ExpenseTypes_ExpenseTypeId",
                table: "LineItems",
                column: "ExpenseTypeId",
                principalTable: "ExpenseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_ExpenseTypes_ExpenseTypeId",
                table: "LineItems");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_ExpenseTypeId",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "ExpenseTypeId",
                table: "LineItems");
        }
    }
}
