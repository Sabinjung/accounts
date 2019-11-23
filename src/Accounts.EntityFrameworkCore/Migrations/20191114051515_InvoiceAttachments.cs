using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class InvoiceAttachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QBOInvoiceId",
                table: "Invoices",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Attachments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_InvoiceId",
                table: "Attachments",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Invoices_InvoiceId",
                table: "Attachments",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Invoices_InvoiceId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_InvoiceId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Attachments");

            migrationBuilder.AlterColumn<int>(
                name: "QBOInvoiceId",
                table: "Invoices",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
