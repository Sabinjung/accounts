using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class InvoiceCycles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_InvoiceCycle_InvoiceCycleId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceCycle",
                table: "InvoiceCycle");

            migrationBuilder.RenameTable(
                name: "InvoiceCycle",
                newName: "InvoiceCycles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceCycles",
                table: "InvoiceCycles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_InvoiceCycles_InvoiceCycleId",
                table: "Projects",
                column: "InvoiceCycleId",
                principalTable: "InvoiceCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_InvoiceCycles_InvoiceCycleId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceCycles",
                table: "InvoiceCycles");

            migrationBuilder.RenameTable(
                name: "InvoiceCycles",
                newName: "InvoiceCycle");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceCycle",
                table: "InvoiceCycle",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_InvoiceCycle_InvoiceCycleId",
                table: "Projects",
                column: "InvoiceCycleId",
                principalTable: "InvoiceCycle",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
