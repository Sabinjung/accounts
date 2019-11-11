using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class Accountsv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "InvoiceCycleId",
                table: "Projects",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AlterColumn<double>(
                name: "Hours",
                table: "HourLogEntries",
                nullable: false,
                oldClrType: typeof(decimal));

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
                name: "FamilyName",
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

            migrationBuilder.CreateTable(
                name: "InvoiceCycle",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceCycle", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_InvoiceCycleId",
                table: "Projects",
                column: "InvoiceCycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_InvoiceCycle_InvoiceCycleId",
                table: "Projects",
                column: "InvoiceCycleId",
                principalTable: "InvoiceCycle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_InvoiceCycle_InvoiceCycleId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "InvoiceCycle");

            migrationBuilder.DropIndex(
                name: "IX_Projects_InvoiceCycleId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "InvoiceCycleId",
                table: "Projects");

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
                name: "FamilyName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConsultantId",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Hours",
                table: "HourLogEntries",
                nullable: false,
                oldClrType: typeof(double));

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
                onDelete: ReferentialAction.Cascade);

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
    }
}
