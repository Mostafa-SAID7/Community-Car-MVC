using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUnmappedCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalMediaUrlsJson",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagsJson",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlsJson",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagsJson",
                table: "Routes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConsJson",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlsJson",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProsJson",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagsJson",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlsJson",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrlsJson",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagsJson",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrerequisitesJson",
                table: "Guides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequiredToolsJson",
                table: "Guides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagsJson",
                table: "Guides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TagsJson",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalMediaUrlsJson",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "TagsJson",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ImageUrlsJson",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "TagsJson",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ConsJson",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ImageUrlsJson",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ProsJson",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TagsJson",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ImageUrlsJson",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "ImageUrlsJson",
                table: "News");

            migrationBuilder.DropColumn(
                name: "TagsJson",
                table: "News");

            migrationBuilder.DropColumn(
                name: "PrerequisitesJson",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "RequiredToolsJson",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "TagsJson",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "TagsJson",
                table: "Groups");
        }
    }
}
