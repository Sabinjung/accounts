using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class Company_Customer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ConsultantName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TermName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Contact2",
                table: "Consultants");

            migrationBuilder.DropColumn(
                name: "BillAddrCity",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BillAddrCountry",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BillAddrCountrySubDivisionCode",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BillAddrLine1",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BillAddrLine2",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BillAddrPostalCode",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "EmailAddresses",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "FamilyName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Fax",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PrimaryPhone",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "Contact1",
                table: "Consultants",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "WebAddress",
                table: "Companies",
                newName: "CustomerId");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConsultantId",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CompanyId",
                table: "Invoices",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ConsultantId",
                table: "Invoices",
                column: "ConsultantId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TermId",
                table: "Invoices",
                column: "TermId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Companies_CompanyId",
                table: "Invoices",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Consultants_ConsultantId",
                table: "Invoices",
                column: "ConsultantId",
                principalTable: "Consultants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Terms_TermId",
                table: "Invoices",
                column: "TermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Companies_CompanyId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Consultants_ConsultantId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Terms_TermId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_CompanyId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ConsultantId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_TermId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ConsultantId",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Consultants",
                newName: "Contact1");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Companies",
                newName: "WebAddress");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConsultantName",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermName",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contact2",
                table: "Consultants",
                type: "varchar(15)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillAddrCity",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillAddrCountry",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillAddrCountrySubDivisionCode",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillAddrLine1",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillAddrLine2",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillAddrPostalCode",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddresses",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FamilyName",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fax",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryPhone",
                table: "Companies",
                nullable: true);
        }
    }
}
