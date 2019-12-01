using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class InvoiceDiscountAmount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Invoices",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Invoices");
        }
    }
}
