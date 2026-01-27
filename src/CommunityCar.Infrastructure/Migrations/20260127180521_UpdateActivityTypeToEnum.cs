using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunityCar.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateActivityTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Convert ActivityType from nvarchar to int
            migrationBuilder.Sql(@"
                -- Add temporary column
                ALTER TABLE UserActivities ADD ActivityTypeTemp int;
                
                -- Convert existing string values to enum integers
                UPDATE UserActivities SET ActivityTypeTemp = 
                    CASE ActivityType
                        WHEN 'View' THEN 1
                        WHEN 'Like' THEN 2
                        WHEN 'Comment' THEN 3
                        WHEN 'Share' THEN 4
                        WHEN 'Follow' THEN 5
                        WHEN 'Unfollow' THEN 6
                        WHEN 'Post' THEN 7
                        WHEN 'Edit' THEN 8
                        WHEN 'Delete' THEN 9
                        WHEN 'Search' THEN 10
                        WHEN 'Login' THEN 11
                        WHEN 'Logout' THEN 12
                        WHEN 'Register' THEN 13
                        WHEN 'ProfileUpdate' THEN 14
                        WHEN 'PasswordChange' THEN 15
                        WHEN 'EmailConfirmation' THEN 16
                        WHEN 'TwoFactorSetup' THEN 17
                        WHEN 'OAuthLink' THEN 18
                        WHEN 'OAuthUnlink' THEN 19
                        WHEN 'BookmarkAdd' THEN 20
                        WHEN 'BookmarkRemove' THEN 21
                        WHEN 'VoteUp' THEN 22
                        WHEN 'VoteDown' THEN 23
                        WHEN 'ReactionAdd' THEN 24
                        WHEN 'ReactionRemove' THEN 25
                        WHEN 'GroupJoin' THEN 26
                        WHEN 'GroupLeave' THEN 27
                        WHEN 'EventAttend' THEN 28
                        WHEN 'EventCancel' THEN 29
                        WHEN 'MessageSend' THEN 30
                        WHEN 'MessageRead' THEN 31
                        WHEN 'NotificationRead' THEN 32
                        WHEN 'SettingsUpdate' THEN 33
                        WHEN 'PrivacyUpdate' THEN 34
                        WHEN 'DataExport' THEN 35
                        WHEN 'DataDelete' THEN 36
                        ELSE 1 -- Default to View
                    END;
                
                -- Drop old column
                ALTER TABLE UserActivities DROP COLUMN ActivityType;
                
                -- Rename temp column
                EXEC sp_rename 'UserActivities.ActivityTypeTemp', 'ActivityType', 'COLUMN';
                
                -- Make column not null
                ALTER TABLE UserActivities ALTER COLUMN ActivityType int NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert ActivityType from int back to nvarchar
            migrationBuilder.Sql(@"
                -- Add temporary column
                ALTER TABLE UserActivities ADD ActivityTypeTemp nvarchar(100);
                
                -- Convert enum integers back to string values
                UPDATE UserActivities SET ActivityTypeTemp = 
                    CASE ActivityType
                        WHEN 1 THEN 'View'
                        WHEN 2 THEN 'Like'
                        WHEN 3 THEN 'Comment'
                        WHEN 4 THEN 'Share'
                        WHEN 5 THEN 'Follow'
                        WHEN 6 THEN 'Unfollow'
                        WHEN 7 THEN 'Post'
                        WHEN 8 THEN 'Edit'
                        WHEN 9 THEN 'Delete'
                        WHEN 10 THEN 'Search'
                        WHEN 11 THEN 'Login'
                        WHEN 12 THEN 'Logout'
                        WHEN 13 THEN 'Register'
                        WHEN 14 THEN 'ProfileUpdate'
                        WHEN 15 THEN 'PasswordChange'
                        WHEN 16 THEN 'EmailConfirmation'
                        WHEN 17 THEN 'TwoFactorSetup'
                        WHEN 18 THEN 'OAuthLink'
                        WHEN 19 THEN 'OAuthUnlink'
                        WHEN 20 THEN 'BookmarkAdd'
                        WHEN 21 THEN 'BookmarkRemove'
                        WHEN 22 THEN 'VoteUp'
                        WHEN 23 THEN 'VoteDown'
                        WHEN 24 THEN 'ReactionAdd'
                        WHEN 25 THEN 'ReactionRemove'
                        WHEN 26 THEN 'GroupJoin'
                        WHEN 27 THEN 'GroupLeave'
                        WHEN 28 THEN 'EventAttend'
                        WHEN 29 THEN 'EventCancel'
                        WHEN 30 THEN 'MessageSend'
                        WHEN 31 THEN 'MessageRead'
                        WHEN 32 THEN 'NotificationRead'
                        WHEN 33 THEN 'SettingsUpdate'
                        WHEN 34 THEN 'PrivacyUpdate'
                        WHEN 35 THEN 'DataExport'
                        WHEN 36 THEN 'DataDelete'
                        ELSE 'View' -- Default to View
                    END;
                
                -- Drop old column
                ALTER TABLE UserActivities DROP COLUMN ActivityType;
                
                -- Rename temp column
                EXEC sp_rename 'UserActivities.ActivityTypeTemp', 'ActivityType', 'COLUMN';
                
                -- Make column not null
                ALTER TABLE UserActivities ALTER COLUMN ActivityType nvarchar(100) NOT NULL;
            ");
        }
    }
}

