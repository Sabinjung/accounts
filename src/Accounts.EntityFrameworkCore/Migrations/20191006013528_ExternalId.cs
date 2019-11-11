using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class ExternalId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "ExternalTermId",
                table: "Terms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalCustomerId",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalTermId",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "ExternalCustomerId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Companies",
                nullable: true);
        }
    }
}
