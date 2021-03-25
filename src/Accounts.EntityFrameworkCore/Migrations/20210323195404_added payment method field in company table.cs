using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class addedpaymentmethodfieldincompanytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_InvoiceCycles_InvoiceCycleId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Terms_TermId",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "TermId",
                table: "Companies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceCycleId",
                table: "Companies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "Companies",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PaymentMethodId",
                table: "Companies",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_InvoiceCycles_InvoiceCycleId",
                table: "Companies",
                column: "InvoiceCycleId",
                principalTable: "InvoiceCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_PaymentMethods_PaymentMethodId",
                table: "Companies",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Terms_TermId",
                table: "Companies",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_InvoiceCycles_InvoiceCycleId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_PaymentMethods_PaymentMethodId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Terms_TermId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_PaymentMethodId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "TermId",
                table: "Companies",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceCycleId",
                table: "Companies",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_InvoiceCycles_InvoiceCycleId",
                table: "Companies",
                column: "InvoiceCycleId",
                principalTable: "InvoiceCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Terms_TermId",
                table: "Companies",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
