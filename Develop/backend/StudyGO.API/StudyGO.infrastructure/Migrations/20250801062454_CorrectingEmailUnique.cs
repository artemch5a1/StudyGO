using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyGO.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectingEmailUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfileEntity_SubjectsEntity_SubjectID",
                table: "UserProfileEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfileEntity_UsersEntity_UserID",
                table: "UserProfileEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfileEntity",
                table: "UserProfileEntity");

            migrationBuilder.RenameTable(
                name: "UserProfileEntity",
                newName: "UserProfilesEntity");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfileEntity_SubjectID",
                table: "UserProfilesEntity",
                newName: "IX_UserProfilesEntity_SubjectID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfilesEntity",
                table: "UserProfilesEntity",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UsersEntity_Email",
                table: "UsersEntity",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfilesEntity_SubjectsEntity_SubjectID",
                table: "UserProfilesEntity",
                column: "SubjectID",
                principalTable: "SubjectsEntity",
                principalColumn: "SubjectID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfilesEntity_UsersEntity_UserID",
                table: "UserProfilesEntity",
                column: "UserID",
                principalTable: "UsersEntity",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfilesEntity_SubjectsEntity_SubjectID",
                table: "UserProfilesEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfilesEntity_UsersEntity_UserID",
                table: "UserProfilesEntity");

            migrationBuilder.DropIndex(
                name: "IX_UsersEntity_Email",
                table: "UsersEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfilesEntity",
                table: "UserProfilesEntity");

            migrationBuilder.RenameTable(
                name: "UserProfilesEntity",
                newName: "UserProfileEntity");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfilesEntity_SubjectID",
                table: "UserProfileEntity",
                newName: "IX_UserProfileEntity_SubjectID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfileEntity",
                table: "UserProfileEntity",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfileEntity_SubjectsEntity_SubjectID",
                table: "UserProfileEntity",
                column: "SubjectID",
                principalTable: "SubjectsEntity",
                principalColumn: "SubjectID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfileEntity_UsersEntity_UserID",
                table: "UserProfileEntity",
                column: "UserID",
                principalTable: "UsersEntity",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
