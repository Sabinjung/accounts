using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class AddedColumnNotesInEndClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndClientId",
                table: "Notes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_EndClientId",
                table: "Notes",
                column: "EndClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_EndClients_EndClientId",
                table: "Notes",
                column: "EndClientId",
                principalTable: "EndClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_EndClients_EndClientId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_EndClientId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "EndClientId",
                table: "Notes");
        }
    }
}
