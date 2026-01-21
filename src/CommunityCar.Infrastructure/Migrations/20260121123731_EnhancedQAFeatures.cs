using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedQAFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AcceptedAnswerId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AnswerCount",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CarEngine",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarMake",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarYear",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAt",
                table: "Questions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClosedBy",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityAt",
                table: "Questions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastActivityBy",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SolvedAt",
                table: "Questions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VoteScore",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfficial",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityAt",
                table: "Groups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MemberCount",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PostCount",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresApproval",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Rules",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "Answers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditReason",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HelpfulCount",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Answers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerifiedByExpert",
                table: "Answers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditedAt",
                table: "Answers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationNote",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "Answers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VerifiedBy",
                table: "Answers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VoteScore",
                table: "Answers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAnswerId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AnswerCount",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CarEngine",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CarMake",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CarYear",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "LastActivityBy",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "SolvedAt",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "VoteScore",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsOfficial",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "MemberCount",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "PostCount",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "RequiresApproval",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Rules",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "EditReason",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "HelpfulCount",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "IsVerifiedByExpert",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "LastEditedAt",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "VerificationNote",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "VoteScore",
                table: "Answers");
        }
    }
}
