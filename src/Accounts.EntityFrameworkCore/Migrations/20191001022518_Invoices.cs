using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class Invoices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvoiceDescriptorId",
                table: "Timesheets",
                newName: "InvoiceId");

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CustomerId = table.Column<int>(nullable: false),
                    TermId = table.Column<int>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    InvoiceDate = table.Column<DateTime>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    TotalHours = table.Column<double>(nullable: false),
                    Rate = table.Column<double>(nullable: false),
                    SubTotal = table.Column<decimal>(nullable: false),
                    IsDiscountPercentageApplied = table.Column<bool>(nullable: false),
                    DiscountPercentage = table.Column<double>(nullable: false),
                    DiscountAmount = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    QBOInvoiceId = table.Column<int>(nullable: false),
                    DocNumber = table.Column<int>(nullable: true),
                    ConsultantId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Consultants_ConsultantId",
                        column: x => x.ConsultantId,
                        principalTable: "Consultants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Terms_TermId",
                        column: x => x.TermId,
                        principalTable: "Terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_InvoiceId",
                table: "Timesheets",
                column: "InvoiceId");

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
                name: "FK_Timesheets_Invoices_InvoiceId",
                table: "Timesheets",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Invoices_InvoiceId",
                table: "Timesheets");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_InvoiceId",
                table: "Timesheets");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "Timesheets",
                newName: "InvoiceDescriptorId");
        }
    }
}
