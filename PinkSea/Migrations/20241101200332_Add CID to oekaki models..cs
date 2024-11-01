using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinkSea.Migrations
{
    /// <inheritdoc />
    public partial class AddCIDtooekakimodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecordCid",
                table: "Oekaki",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordCid",
                table: "Oekaki");
        }
    }
}
