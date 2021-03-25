using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class addedinvoicecyclefieldincompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.AddColumn<int>(
                name: "InvoiceCycleId",
                table: "Companies",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_InvoiceCycleId",
                table: "Companies",
                column: "InvoiceCycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_InvoiceCycles_InvoiceCycleId",
                table: "Companies",
                column: "InvoiceCycleId",
                principalTable: "InvoiceCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

        
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_InvoiceCycles_InvoiceCycleId",
                table: "Companies");


            migrationBuilder.DropIndex(
                name: "IX_Companies_InvoiceCycleId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "InvoiceCycleId",
                table: "Companies");

     

        }
    }
}
