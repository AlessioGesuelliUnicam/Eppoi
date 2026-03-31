using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eppoi.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasCompletedQuestionnaire",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Interests",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileVector",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeAvailability",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TravelStyle",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasCompletedQuestionnaire",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Interests",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileVector",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TimeAvailability",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TravelStyle",
                table: "Users");
        }
    }
}
