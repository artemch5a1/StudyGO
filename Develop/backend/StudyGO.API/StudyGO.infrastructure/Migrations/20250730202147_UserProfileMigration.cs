using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyGO.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserProfileMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorProfilesEntity_FormatsEntity_FormatID",
                table: "TutorProfilesEntity");

            migrationBuilder.CreateTable(
                name: "SubjectsEntity",
                columns: table => new
                {
                    SubjectID = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectsEntity", x => x.SubjectID);
                });

            migrationBuilder.CreateTable(
                name: "UserProfileEntity",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uuid", nullable: false),
                    DateBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FavoriteSubjectSubjectID = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileEntity", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_UserProfileEntity_SubjectsEntity_FavoriteSubjectSubjectID",
                        column: x => x.FavoriteSubjectSubjectID,
                        principalTable: "SubjectsEntity",
                        principalColumn: "SubjectID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserProfileEntity_UsersEntity_UserID",
                        column: x => x.UserID,
                        principalTable: "UsersEntity",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileEntity_FavoriteSubjectSubjectID",
                table: "UserProfileEntity",
                column: "FavoriteSubjectSubjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorProfilesEntity_FormatsEntity_FormatID",
                table: "TutorProfilesEntity",
                column: "FormatID",
                principalTable: "FormatsEntity",
                principalColumn: "FormatID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorProfilesEntity_FormatsEntity_FormatID",
                table: "TutorProfilesEntity");

            migrationBuilder.DropTable(
                name: "UserProfileEntity");

            migrationBuilder.DropTable(
                name: "SubjectsEntity");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorProfilesEntity_FormatsEntity_FormatID",
                table: "TutorProfilesEntity",
                column: "FormatID",
                principalTable: "FormatsEntity",
                principalColumn: "FormatID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
