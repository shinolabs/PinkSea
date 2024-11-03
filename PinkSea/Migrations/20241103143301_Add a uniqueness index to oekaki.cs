using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinkSea.Migrations
{
    /// <inheritdoc />
    public partial class Addauniquenessindextooekaki : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Oekaki_AuthorDid",
                table: "Oekaki");

            migrationBuilder.CreateIndex(
                name: "IX_Oekaki_AuthorDid_OekakiTid",
                table: "Oekaki",
                columns: new[] { "AuthorDid", "OekakiTid" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Oekaki_AuthorDid_OekakiTid",
                table: "Oekaki");

            migrationBuilder.CreateIndex(
                name: "IX_Oekaki_AuthorDid",
                table: "Oekaki",
                column: "AuthorDid");
        }
    }
}
