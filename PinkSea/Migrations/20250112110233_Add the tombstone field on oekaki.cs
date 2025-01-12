using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinkSea.Migrations
{
    /// <inheritdoc />
    public partial class Addthetombstonefieldonoekaki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Tombstone",
                table: "Oekaki",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Oekaki_Tombstone",
                table: "Oekaki",
                column: "Tombstone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Oekaki_Tombstone",
                table: "Oekaki");

            migrationBuilder.DropColumn(
                name: "Tombstone",
                table: "Oekaki");
        }
    }
}
