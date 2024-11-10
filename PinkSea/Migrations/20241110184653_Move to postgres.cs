using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PinkSea.Migrations
{
    /// <inheritdoc />
    public partial class Movetopostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientPrivateKey = table.Column<string>(type: "text", nullable: false),
                    ClientPublicKey = table.Column<string>(type: "text", nullable: false),
                    KeyId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OAuthStates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Json = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Did = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Did);
                });

            migrationBuilder.CreateTable(
                name: "Oekaki",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false),
                    OekakiTid = table.Column<string>(type: "text", nullable: false),
                    AuthorDid = table.Column<string>(type: "text", nullable: false),
                    IndexedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RecordCid = table.Column<string>(type: "text", nullable: false),
                    BlobCid = table.Column<string>(type: "text", nullable: false),
                    AltText = table.Column<string>(type: "text", nullable: true),
                    IsNsfw = table.Column<bool>(type: "boolean", nullable: true),
                    ParentId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oekaki", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Oekaki_Oekaki_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Oekaki",
                        principalColumn: "Key");
                    table.ForeignKey(
                        name: "FK_Oekaki_Users_AuthorDid",
                        column: x => x.AuthorDid,
                        principalTable: "Users",
                        principalColumn: "Did",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagOekakiRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OekakiId = table.Column<string>(type: "text", nullable: false),
                    TagId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagOekakiRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagOekakiRelations_Oekaki_OekakiId",
                        column: x => x.OekakiId,
                        principalTable: "Oekaki",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagOekakiRelations_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Oekaki_AuthorDid_OekakiTid",
                table: "Oekaki",
                columns: new[] { "AuthorDid", "OekakiTid" });

            migrationBuilder.CreateIndex(
                name: "IX_Oekaki_ParentId",
                table: "Oekaki",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TagOekakiRelations_OekakiId",
                table: "TagOekakiRelations",
                column: "OekakiId");

            migrationBuilder.CreateIndex(
                name: "IX_TagOekakiRelations_TagId",
                table: "TagOekakiRelations",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "OAuthStates");

            migrationBuilder.DropTable(
                name: "TagOekakiRelations");

            migrationBuilder.DropTable(
                name: "Oekaki");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
