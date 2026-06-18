using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostPilot.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryCaptionTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CaptionTemplate",
                table: "categories",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaptionTemplate",
                table: "categories");
        }
    }
}
