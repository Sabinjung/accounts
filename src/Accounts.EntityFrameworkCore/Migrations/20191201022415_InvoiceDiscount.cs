using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class InvoiceDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DocNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsDiscountPercentageApplied",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Invoices");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountValue",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiscountType",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountValue",
                table: "Invoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                table: "Invoices");

            migrationBuilder.AlterColumn<double>(
                name: "DiscountValue",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Invoices",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "DiscountPercentage",
                table: "Invoices",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "DocNumber",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscountPercentageApplied",
                table: "Invoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);
        }
    }
}
