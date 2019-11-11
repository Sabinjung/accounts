using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class NameResolve : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Projects_ProjectId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Timesheets_TimesheetId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetStatus_StatusId",
                table: "Timesheets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimesheetStatus",
                table: "TimesheetStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment");

            migrationBuilder.RenameTable(
                name: "TimesheetStatus",
                newName: "TimesheetStatuses");

            migrationBuilder.RenameTable(
                name: "Attachment",
                newName: "Attachments");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_TimesheetId",
                table: "Attachments",
                newName: "IX_Attachments_TimesheetId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_ProjectId",
                table: "Attachments",
                newName: "IX_Attachments_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimesheetStatuses",
                table: "TimesheetStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Projects_ProjectId",
                table: "Attachments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Timesheets_TimesheetId",
                table: "Attachments",
                column: "TimesheetId",
                principalTable: "Timesheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetStatuses_StatusId",
                table: "Timesheets",
                column: "StatusId",
                principalTable: "TimesheetStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Projects_ProjectId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Timesheets_TimesheetId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_TimesheetStatuses_StatusId",
                table: "Timesheets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TimesheetStatuses",
                table: "TimesheetStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments");

            migrationBuilder.RenameTable(
                name: "TimesheetStatuses",
                newName: "TimesheetStatus");

            migrationBuilder.RenameTable(
                name: "Attachments",
                newName: "Attachment");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_TimesheetId",
                table: "Attachment",
                newName: "IX_Attachment_TimesheetId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_ProjectId",
                table: "Attachment",
                newName: "IX_Attachment_ProjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TimesheetStatus",
                table: "TimesheetStatus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Projects_ProjectId",
                table: "Attachment",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Timesheets_TimesheetId",
                table: "Attachment",
                column: "TimesheetId",
                principalTable: "Timesheets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_TimesheetStatus_StatusId",
                table: "Timesheets",
                column: "StatusId",
                principalTable: "TimesheetStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
