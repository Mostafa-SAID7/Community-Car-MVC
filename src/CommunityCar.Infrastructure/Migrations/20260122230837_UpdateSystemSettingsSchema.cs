using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSystemSettingsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsEncrypted",
                table: "SystemSettings",
                newName: "IsSecure");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DataType",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultValue",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReadOnly",
                table: "SystemSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ValidationRules",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "DataType",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "DefaultValue",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "IsReadOnly",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ValidationRules",
                table: "SystemSettings");

            migrationBuilder.RenameColumn(
                name: "IsSecure",
                table: "SystemSettings",
                newName: "IsEncrypted");
        }
    }
}
