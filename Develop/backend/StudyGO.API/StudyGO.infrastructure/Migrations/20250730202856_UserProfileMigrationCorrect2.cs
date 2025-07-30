using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyGO.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserProfileMigrationCorrect2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfileEntity_SubjectsEntity_FavoriteSubjectSubjectID",
                table: "UserProfileEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserProfileEntity_FavoriteSubjectSubjectID",
                table: "UserProfileEntity");

            migrationBuilder.DropColumn(
                name: "FavoriteSubjectSubjectID",
                table: "UserProfileEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectID",
                table: "UserProfileEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileEntity_SubjectID",
                table: "UserProfileEntity",
                column: "SubjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfileEntity_SubjectsEntity_SubjectID",
                table: "UserProfileEntity",
                column: "SubjectID",
                principalTable: "SubjectsEntity",
                principalColumn: "SubjectID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfileEntity_SubjectsEntity_SubjectID",
                table: "UserProfileEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserProfileEntity_SubjectID",
                table: "UserProfileEntity");

            migrationBuilder.DropColumn(
                name: "SubjectID",
                table: "UserProfileEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "FavoriteSubjectSubjectID",
                table: "UserProfileEntity",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileEntity_FavoriteSubjectSubjectID",
                table: "UserProfileEntity",
                column: "FavoriteSubjectSubjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfileEntity_SubjectsEntity_FavoriteSubjectSubjectID",
                table: "UserProfileEntity",
                column: "FavoriteSubjectSubjectID",
                principalTable: "SubjectsEntity",
                principalColumn: "SubjectID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
