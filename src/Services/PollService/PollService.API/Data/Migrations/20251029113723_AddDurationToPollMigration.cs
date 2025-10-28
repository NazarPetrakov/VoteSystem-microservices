using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollService.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationToPollMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DurationInSeconds",
                table: "Polls",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationInSeconds",
                table: "Polls");
        }
    }
}
