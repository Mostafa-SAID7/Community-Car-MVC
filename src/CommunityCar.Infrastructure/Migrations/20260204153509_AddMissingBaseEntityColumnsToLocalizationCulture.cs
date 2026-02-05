using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingBaseEntityColumnsToLocalizationCulture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LocalizationResources",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LocalizationResources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "LocalizationResources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "LocalizationResources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LocalizationResources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LocalizationResources",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "LocalizationResources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Exception",
                table: "ErrorLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "ErrorLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "ErrorLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LocalizationResources");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LocalizationResources");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "LocalizationResources");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "LocalizationResources");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LocalizationResources");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LocalizationResources");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LocalizationResources");

            migrationBuilder.DropColumn(
                name: "ShareCount",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Exception",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "ErrorLogs");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "ErrorLogs");
        }
    }
}
