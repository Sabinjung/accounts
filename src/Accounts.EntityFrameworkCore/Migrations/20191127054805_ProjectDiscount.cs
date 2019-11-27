using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class ProjectDiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDiscountPercentageApplied",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "DiscountPercentage",
                table: "Projects",
                newName: "DiscountValue");

            migrationBuilder.AddColumn<int>(
                name: "DiscountType",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "DiscountValue",
                table: "Projects",
                newName: "DiscountPercentage");

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscountPercentageApplied",
                table: "Projects",
                nullable: false,
                defaultValue: false);
        }
    }
}
