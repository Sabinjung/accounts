using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class ProjectTableModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_EndClients_EndClientId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_EndClientId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "EndClientId",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "EndClient",
                table: "Projects",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndClient",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "EndClientId",
                table: "Projects",
                nullable: true);

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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
