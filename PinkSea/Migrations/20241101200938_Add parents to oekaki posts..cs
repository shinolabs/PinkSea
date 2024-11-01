using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinkSea.Migrations
{
    /// <inheritdoc />
    public partial class Addparentstooekakiposts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "Oekaki",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Oekaki_ParentId",
                table: "Oekaki",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Oekaki_Oekaki_ParentId",
                table: "Oekaki",
                column: "ParentId",
                principalTable: "Oekaki",
                principalColumn: "Tid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Oekaki_Oekaki_ParentId",
                table: "Oekaki");

            migrationBuilder.DropIndex(
                name: "IX_Oekaki_ParentId",
                table: "Oekaki");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Oekaki");
        }
    }
}
