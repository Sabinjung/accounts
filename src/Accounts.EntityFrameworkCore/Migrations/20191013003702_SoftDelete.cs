using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Accounts.Migrations
{
    public partial class SoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Timesheets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Timesheets",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Timesheets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "TimesheetPeriods",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "TimesheetPeriods",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TimesheetPeriods",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Terms",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Terms",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Terms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Projects",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Invoices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Invoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "HourLogEntries",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "HourLogEntries",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HourLogEntries",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Consultants",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Consultants",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Consultants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Companies",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Companies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "Attachments",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Attachments",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Attachments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "TimesheetPeriods");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "TimesheetPeriods");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TimesheetPeriods");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "HourLogEntries");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "HourLogEntries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HourLogEntries");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Consultants");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Consultants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Consultants");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Attachments");
        }
    }
}
