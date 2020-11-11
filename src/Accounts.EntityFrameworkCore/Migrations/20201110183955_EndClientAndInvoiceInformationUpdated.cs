using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class EndClientAndInvoiceInformationUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndClientId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndClientName",
                table: "Invoices",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EndClients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    ClientName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndClients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_EndClientId",
                table: "Projects",
                column: "EndClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_EndClients_EndClientId",
                table: "Projects",
                column: "EndClientId",
                principalTable: "EndClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_EndClients_EndClientId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "EndClients");

            migrationBuilder.DropIndex(
                name: "IX_Projects_EndClientId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "EndClientId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "EndClientName",
                table: "Invoices");
        }
    }
}
