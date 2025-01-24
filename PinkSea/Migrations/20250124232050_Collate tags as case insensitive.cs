using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinkSea.Migrations
{
    /// <inheritdoc />
    public partial class Collatetagsascaseinsensitive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case-insensitive", "und-u-ks-level2,und-u-ks-level2,icu,False");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "text",
                nullable: false,
                collation: "case-insensitive",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "TagId",
                table: "TagOekakiRelations",
                type: "text",
                nullable: false,
                collation: "case-insensitive",
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:CollationDefinition:case-insensitive", "und-u-ks-level2,und-u-ks-level2,icu,False");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "case-insensitive");

            migrationBuilder.AlterColumn<string>(
                name: "TagId",
                table: "TagOekakiRelations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldCollation: "case-insensitive");
        }
    }
}
