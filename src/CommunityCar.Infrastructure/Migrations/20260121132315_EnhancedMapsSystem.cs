using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedMapsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "PointsOfInterest",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "PointsOfInterest",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowCheckIns",
                table: "PointsOfInterest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowReviews",
                table: "PointsOfInterest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Amenities",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "PointsOfInterest",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "PointsOfInterest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CheckInCount",
                table: "PointsOfInterest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentAttendees",
                table: "PointsOfInterest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EventEndTime",
                table: "PointsOfInterest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EventStartTime",
                table: "PointsOfInterest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen24Hours",
                table: "PointsOfInterest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "PointsOfInterest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReported",
                table: "PointsOfInterest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTemporarilyClosed",
                table: "PointsOfInterest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "PointsOfInterest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxAttendees",
                table: "PointsOfInterest",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpeningHours",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethods",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceRange",
                table: "PointsOfInterest",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PricingInfo",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "PointsOfInterest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "PointsOfInterest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Services",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SupportedVehicleTypes",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "PointsOfInterest",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VerifiedBy",
                table: "PointsOfInterest",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "PointsOfInterest",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Guides",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BookmarkCount",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Guides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Guides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedMinutes",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Guides",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Guides",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedAt",
                table: "Guides",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "Guides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Guides",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Guides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CheckIns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PointOfInterestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false),
                    CheckInLatitude = table.Column<double>(type: "float", nullable: true),
                    CheckInLongitude = table.Column<double>(type: "float", nullable: true),
                    DistanceFromPOI = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckIns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    DistanceKm = table.Column<double>(type: "float", nullable: false),
                    EstimatedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    TimesCompleted = table.Column<int>(type: "int", nullable: false),
                    IsScenic = table.Column<bool>(type: "bit", nullable: false),
                    HasTolls = table.Column<bool>(type: "bit", nullable: false),
                    IsOffRoad = table.Column<bool>(type: "bit", nullable: false),
                    SurfaceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BestTimeToVisit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SafetyNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastConditionUpdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WaypointsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckIns");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "AllowCheckIns",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "AllowReviews",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Amenities",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "CheckInCount",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "CurrentAttendees",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "EventEndTime",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "EventStartTime",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "IsOpen24Hours",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "IsReported",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "IsTemporarilyClosed",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "MaxAttendees",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "OpeningHours",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "PaymentMethods",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "PriceRange",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "PricingInfo",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Services",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "SupportedVehicleTypes",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "PointsOfInterest");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "BookmarkCount",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "EstimatedMinutes",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "PublishedAt",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "Summary",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Guides");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "PointsOfInterest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
