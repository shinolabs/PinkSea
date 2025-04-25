using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinkSea.Migrations
{
    /// <inheritdoc />
    public partial class Addrepostatusandappviewblockedstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AppViewBlocked",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RepoStatus",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppViewBlocked",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RepoStatus",
                table: "Users");
        }
    }
}
