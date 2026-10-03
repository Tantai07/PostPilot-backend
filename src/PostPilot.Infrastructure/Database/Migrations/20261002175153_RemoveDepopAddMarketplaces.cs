using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostPilot.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepopAddMarketplaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE meta_tokens
                SET "IsDeleted" = TRUE, "DeletedAt" = NOW()
                WHERE "SocialAccountId" IN (
                    SELECT "Id" FROM social_accounts WHERE "Platform" = 'Depop'
                );

                UPDATE social_accounts
                SET "IsDeleted" = TRUE, "DeletedAt" = NOW()
                WHERE "Platform" = 'Depop';

                UPDATE profiles
                SET "DefaultTargets" = CASE
                    WHEN "DefaultTargets" = 'Depop' THEN NULL
                    ELSE replace(replace("DefaultTargets", 'Depop,', ''), ',Depop', '')
                END
                WHERE "DefaultTargets" LIKE '%Depop%';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE social_accounts
                SET "IsDeleted" = FALSE, "DeletedAt" = NULL
                WHERE "Platform" = 'Depop';

                UPDATE meta_tokens
                SET "IsDeleted" = FALSE, "DeletedAt" = NULL
                WHERE "SocialAccountId" IN (
                    SELECT "Id" FROM social_accounts WHERE "Platform" = 'Depop'
                );
                """);
        }
    }
}
