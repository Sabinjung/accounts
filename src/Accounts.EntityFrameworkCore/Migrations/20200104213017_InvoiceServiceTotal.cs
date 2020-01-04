using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class InvoiceServiceTotal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ServiceTotal",
                table: "Invoices",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceTotal",
                table: "Invoices");
        }
    }
}
