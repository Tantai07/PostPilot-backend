using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostPilot.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceGrailedWithDepop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE profiles
                SET "DefaultTargets" = replace("DefaultTargets", 'Grailed', 'Depop')
                WHERE "DefaultTargets" LIKE '%Grailed%';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE profiles
                SET "DefaultTargets" = replace("DefaultTargets", 'Depop', 'Grailed')
                WHERE "DefaultTargets" LIKE '%Depop%';
                """);
        }
    }
}
