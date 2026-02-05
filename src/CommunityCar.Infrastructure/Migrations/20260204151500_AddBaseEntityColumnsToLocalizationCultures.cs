using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityColumnsToLocalizationCultures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "LocalizationCultures",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LocalizationCultures",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LocalizationCultures",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LocalizationCultures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "LocalizationCultures",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LocalizationCultures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "LocalizationCultures",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "LocalizationCultures",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "LocalizationCultures");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LocalizationCultures");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LocalizationCultures");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LocalizationCultures");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LocalizationCultures");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LocalizationCultures");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "LocalizationCultures");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "LocalizationCultures");
        }
    }
}