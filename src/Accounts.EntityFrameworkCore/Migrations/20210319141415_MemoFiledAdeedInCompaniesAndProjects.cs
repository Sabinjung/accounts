using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class MemoFiledAdeedInCompaniesAndProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Memo",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Companies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Memo",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Companies");
        }
    }
}
