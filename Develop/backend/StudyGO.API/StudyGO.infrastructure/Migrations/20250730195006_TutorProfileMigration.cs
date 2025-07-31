using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace StudyGO.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TutorProfileMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormatsEntity",
                columns: table => new
                {
                    FormatID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormatsEntity", x => x.FormatID);
                });

            migrationBuilder.CreateTable(
                name: "TutorProfilesEntity",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uuid", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    PricePerHour = table.Column<decimal>(type: "numeric", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    FormatID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorProfilesEntity", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_TutorProfilesEntity_FormatsEntity_FormatID",
                        column: x => x.FormatID,
                        principalTable: "FormatsEntity",
                        principalColumn: "FormatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TutorProfilesEntity_UsersEntity_UserID",
                        column: x => x.UserID,
                        principalTable: "UsersEntity",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutorProfilesEntity_FormatID",
                table: "TutorProfilesEntity",
                column: "FormatID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TutorProfilesEntity");

            migrationBuilder.DropTable(
                name: "FormatsEntity");
        }
    }
}
