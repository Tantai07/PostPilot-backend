using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostPilot.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class ExpandOAuthConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EncryptedAccessToken",
                table: "meta_tokens",
                type: "character varying(8192)",
                maxLength: 8192,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096);

            migrationBuilder.AddColumn<string>(
                name: "EncryptedRefreshToken",
                table: "meta_tokens",
                type: "character varying(8192)",
                maxLength: 8192,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Scope",
                table: "meta_tokens",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedRefreshToken",
                table: "meta_tokens");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "meta_tokens");

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedAccessToken",
                table: "meta_tokens",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8192)",
                oldMaxLength: 8192);
        }
    }
}
