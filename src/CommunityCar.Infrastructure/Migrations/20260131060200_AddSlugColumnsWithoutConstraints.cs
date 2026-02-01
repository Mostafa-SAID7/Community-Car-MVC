using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugColumnsWithoutConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Stories table first
            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false),
                    ReplyCount = table.Column<int>(type: "int", nullable: false),
                    ShareCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsHighlighted = table.Column<bool>(type: "bit", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    LocationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarMake = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarYear = table.Column<int>(type: "int", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    AllowReplies = table.Column<bool>(type: "bit", nullable: false),
                    AllowSharing = table.Column<bool>(type: "bit", nullable: false),
                    TagsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MentionedUsersJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalMediaUrlsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                });

            // Add Slug columns without unique constraints first
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Stories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Guides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Groups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Events",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            // Populate existing records with generated slugs
            migrationBuilder.Sql(@"
                UPDATE Posts 
                SET Slug = 'post-' + CAST(Id AS VARCHAR(36))
                WHERE Slug IS NULL OR Slug = '';
            ");

            migrationBuilder.Sql(@"
                UPDATE Stories 
                SET Slug = 'story-' + CAST(Id AS VARCHAR(36))
                WHERE Slug IS NULL OR Slug = '';
            ");

            migrationBuilder.Sql(@"
                UPDATE Groups 
                SET Slug = 'group-' + CAST(Id AS VARCHAR(36))
                WHERE Slug IS NULL OR Slug = '';
            ");

            migrationBuilder.Sql(@"
                UPDATE Events 
                SET Slug = 'event-' + CAST(Id AS VARCHAR(36))
                WHERE Slug IS NULL OR Slug = '';
            ");

            migrationBuilder.Sql(@"
                UPDATE Guides 
                SET Slug = 'guide-' + CAST(Id AS VARCHAR(36))
                WHERE Slug IS NULL OR Slug = '';
            ");

            // Now make the columns NOT NULL and add unique constraints
            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Stories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Posts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Guides",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Groups",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Events",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            // Create unique indexes
            migrationBuilder.CreateIndex(
                name: "IX_Stories_Slug",
                table: "Stories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guides_Slug",
                table: "Guides",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Slug",
                table: "Groups",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_Slug",
                table: "Events",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stories_Slug",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Slug",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Guides_Slug",
                table: "Guides");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Slug",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Events_Slug",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Guides");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Stories");
        }
    }
}