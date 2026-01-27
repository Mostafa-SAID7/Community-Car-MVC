using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityLocalizationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommentAr",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyAr",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentAr",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyAr",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeadlineAr",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SummaryAr",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationDetailsAr",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleAr",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyAr",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentAr",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "BodyAr",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ContentAr",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "BodyAr",
                table: "News");

            migrationBuilder.DropColumn(
                name: "HeadlineAr",
                table: "News");

            migrationBuilder.DropColumn(
                name: "SummaryAr",
                table: "News");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationDetailsAr",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TitleAr",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "BodyAr",
                table: "Answers");
        }
    }
}

