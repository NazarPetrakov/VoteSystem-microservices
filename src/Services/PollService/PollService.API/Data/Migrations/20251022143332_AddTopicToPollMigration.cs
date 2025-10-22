using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollService.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicToPollMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Polls");

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "Polls",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Polls");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Polls",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
