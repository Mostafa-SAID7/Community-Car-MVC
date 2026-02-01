using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Ensure Stories table exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stories')
                BEGIN
                    CREATE TABLE [Stories] (
                        [Id] uniqueidentifier NOT NULL,
                        [MediaUrl] nvarchar(max) NOT NULL,
                        [AuthorId] uniqueidentifier NOT NULL,
                        [ExpiresAt] datetime2 NOT NULL,
                        [Caption] nvarchar(max) NULL,
                        [CaptionAr] nvarchar(max) NULL,
                        [Type] int NOT NULL,
                        [ThumbnailUrl] nvarchar(max) NULL,
                        [Duration] int NOT NULL,
                        [ViewCount] int NOT NULL,
                        [LikeCount] int NOT NULL,
                        [ReplyCount] int NOT NULL,
                        [ShareCount] int NOT NULL,
                        [IsActive] bit NOT NULL,
                        [IsArchived] bit NOT NULL,
                        [IsFeatured] bit NOT NULL,
                        [IsHighlighted] bit NOT NULL,
                        [Latitude] float NULL,
                        [Longitude] float NULL,
                        [LocationName] nvarchar(max) NULL,
                        [CarMake] nvarchar(max) NULL,
                        [CarModel] nvarchar(max) NULL,
                        [CarYear] int NULL,
                        [EventType] nvarchar(max) NULL,
                        [Visibility] int NOT NULL,
                        [AllowReplies] bit NOT NULL,
                        [AllowSharing] bit NOT NULL,
                        [TagsJson] nvarchar(max) NOT NULL,
                        [AdditionalMediaUrlsJson] nvarchar(max) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        [CreatedBy] nvarchar(max) NULL,
                        [UpdatedBy] nvarchar(max) NULL,
                        [IsDeleted] bit NOT NULL,
                        [DeletedAt] datetime2 NULL,
                        [DeletedBy] nvarchar(max) NULL,
                        CONSTRAINT [PK_Stories] PRIMARY KEY ([Id])
                    );
                END
            ");

            // 2. Add nullable Slug columns to all relevant tables if they don't exist
            var tablesWithMax100 = new[] { "Stories", "Posts", "Guides", "Groups", "Events", "News" };
            foreach (var table in tablesWithMax100)
            {
                migrationBuilder.Sql($@"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Slug' AND Object_ID = Object_ID(N'{table}'))
                    BEGIN
                        ALTER TABLE [{table}] ADD [Slug] nvarchar(100) NULL;
                    END
                ");
            }

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Slug' AND Object_ID = Object_ID(N'Questions'))
                BEGIN
                    ALTER TABLE [Questions] ADD [Slug] nvarchar(150) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Slug' AND Object_ID = Object_ID(N'Categories'))
                BEGIN
                    ALTER TABLE [Categories] ADD [Slug] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Slug' AND Object_ID = Object_ID(N'Tags'))
                BEGIN
                    ALTER TABLE [Tags] ADD [Slug] nvarchar(max) NULL;
                END
            ");

            // 3. Populate slugs for existing records that have NULL or empty Slug
            migrationBuilder.Sql(@"
                UPDATE Stories SET Slug = 'story-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
                UPDATE Posts SET Slug = 'post-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
                UPDATE Guides SET Slug = 'guide-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
                UPDATE Groups SET Slug = 'group-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
                UPDATE Events SET Slug = 'event-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
                UPDATE News SET Slug = 'news-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
                UPDATE Questions SET Slug = 'question-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
                UPDATE Categories SET Slug = 'cat-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
                UPDATE Tags SET Slug = 'tag-' + CAST(Id AS VARCHAR(36)) WHERE Slug IS NULL OR Slug = '';
            ");

            // 4. Make columns non-nullable and add unique indexes where appropriate
            foreach (var table in tablesWithMax100)
            {
                migrationBuilder.Sql($@"ALTER TABLE [{table}] ALTER COLUMN [Slug] nvarchar(100) NOT NULL;");
                migrationBuilder.Sql($@"
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_{table}_Slug' AND object_id = OBJECT_ID(N'{table}'))
                    BEGIN
                        CREATE UNIQUE INDEX [IX_{table}_Slug] ON [{table}] ([Slug]);
                    END
                ");
            }

            migrationBuilder.Sql(@"ALTER TABLE [Questions] ALTER COLUMN [Slug] nvarchar(150) NOT NULL;");
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Questions_Slug' AND object_id = OBJECT_ID(N'Questions'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_Questions_Slug] ON [Questions] ([Slug]);
                END
            ");

            // Categories and Tags use MAX by default in some places or large values, but we'll try to keep them NOT NULL
            migrationBuilder.Sql(@"ALTER TABLE [Categories] ALTER COLUMN [Slug] nvarchar(max) NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE [Tags] ALTER COLUMN [Slug] nvarchar(max) NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // For repair migrations, Down is best left as a best-effort or no-op to avoid breaking things further
        }
    }
}
