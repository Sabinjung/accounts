using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class AddedExternalProjectGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExternalProjectGuid",
                table: "Projects",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalProjectGuid",
                table: "Projects");
        }
    }
}
