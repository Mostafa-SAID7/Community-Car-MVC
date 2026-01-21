using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedNewsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowReplies",
                table: "Stories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowSharing",
                table: "Stories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarMake",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarYear",
                table: "Stories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Stories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Stories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Stories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHighlighted",
                table: "Stories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Stories",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Stories",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReplyCount",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Stories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Visibility",
                table: "Stories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CarMake",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarYear",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComfortRating",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HelpfulCount",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecommended",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedPurchase",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Mileage",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotHelpfulCount",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "OwnershipDuration",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerformanceRating",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Reviews",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QualityRating",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReliabilityRating",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReplyCount",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ValueRating",
                table: "Reviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "News",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "CarMake",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarYear",
                table: "News",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CommentCount",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "News",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "News",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "News",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShareCount",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceUrl",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowReplies",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "AllowSharing",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Caption",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "CarMake",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "CarYear",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "IsHighlighted",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ReplyCount",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ShareCount",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "CarMake",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CarYear",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ComfortRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "EditedAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "HelpfulCount",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsRecommended",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsVerifiedPurchase",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Mileage",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "NotHelpfulCount",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "OwnershipDuration",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PerformanceRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "QualityRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReliabilityRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReplyCount",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ValueRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CarMake",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CarYear",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "News");

            migrationBuilder.DropColumn(
                name: "CommentCount",
                table: "News");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "News");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "News");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "News");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "News");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "News");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ShareCount",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "News");

            migrationBuilder.DropColumn(
                name: "SourceUrl",
                table: "News");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "News");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "News");
        }
    }
}
